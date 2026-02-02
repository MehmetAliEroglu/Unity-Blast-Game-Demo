using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class BoardManager : MonoBehaviour
{
    [Header("References")]
    // NOTE: removed 'LevelData' from Inspector because GameManager will provide it.
    [SerializeField] private BlockStyle[] blockStyles;

    private LevelData levelData;
    private Block[,] allBlocks;
    private bool isRefilling = false;

    public Action<int> OnBlockBlasted;
    public bool inputLocked = false;

    // REPLACED 'Start' with 'Initialize'
    // This allows GameManager to pass the specific LevelData asset.
    public void Initialize(LevelData data)
    {
        this.levelData = data;

        // Initialize Grid Array
        allBlocks = new Block[levelData.columns, levelData.rows];

        GenerateGrid();
        AdjustCamera();

        // Check for initial deadlock
        if (DeadlockSolver.IsDeadlocked(allBlocks, levelData.columns, levelData.rows))
        {
            DeadlockSolver.SolveDeadlock(allBlocks, levelData.columns, levelData.rows, levelData);
        }

        UpdateBoardVisuals();
    }

    private void Update()
    {
        if (isRefilling || inputLocked) return;

        if (Input.GetMouseButtonDown(0))
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Block clickedBlock = hit.collider.GetComponent<Block>();
            if (clickedBlock != null)
            {
                StartCoroutine(BlastRoutine(clickedBlock));
            }
        }
    }

    private IEnumerator BlastRoutine(Block block)
    {
        List<Block> matches = GetConnectedBlocks(block);

        if (matches.Count >= 2)
        {
            isRefilling = true;

            foreach (Block b in matches)
            {
                RemoveBlock(b);
            }

            // Notify GameManager about the blast count
            OnBlockBlasted?.Invoke(matches.Count);

            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(ApplyGravity());

            // Update visuals after gravity settles
            UpdateBoardVisuals();

            // Check for deadlock after the move
            if (DeadlockSolver.IsDeadlocked(allBlocks, levelData.columns, levelData.rows))
            {
                yield return new WaitForSeconds(0.5f); // Small delay for player to see
                DeadlockSolver.SolveDeadlock(allBlocks, levelData.columns, levelData.rows, levelData);
                UpdateBoardVisuals();
            }

            isRefilling = false;
        }
    }

    private void UpdateBoardVisuals()
    {
        bool[,] visited = new bool[levelData.columns, levelData.rows];

        for (int x = 0; x < levelData.columns; x++)
        {
            for (int y = 0; y < levelData.rows; y++)
            {
                Block currentBlock = allBlocks[x, y];

                if (currentBlock == null || visited[x, y]) continue;

                List<Block> group = GetConnectedBlocks(currentBlock);
                Sprite spriteToUse = GetSpriteForGroupSize(currentBlock.colorIndex, group.Count);

                foreach (Block member in group)
                {
                    member.GetComponent<SpriteRenderer>().sprite = spriteToUse;
                    visited[member.x, member.y] = true;
                }
            }
        }
    }

    private Sprite GetSpriteForGroupSize(int colorIndex, int groupSize)
    {
        BlockStyle style = blockStyles[colorIndex];

        if (groupSize > levelData.thresholdC) return style.iconC;
        if (groupSize > levelData.thresholdB) return style.iconB;
        if (groupSize > levelData.thresholdA) return style.iconA;

        return style.iconDefault;
    }

    private IEnumerator ApplyGravity()
    {
        for (int x = 0; x < levelData.columns; x++)
        {
            int writeIndex = 0;

            // 1. Move existing blocks down
            for (int y = 0; y < levelData.rows; y++)
            {
                if (allBlocks[x, y] != null)
                {
                    if (y != writeIndex)
                    {
                        Block blockToMove = allBlocks[x, y];
                        allBlocks[x, writeIndex] = blockToMove;
                        allBlocks[x, y] = null;

                        blockToMove.MoveToPosition(new Vector2(x, writeIndex), 0.4f);
                        blockToMove.Init(x, writeIndex, blockToMove.colorIndex, blockStyles[blockToMove.colorIndex].iconDefault);
                    }
                    writeIndex++;
                }
            }

            // 2. Spawn new blocks
            for (int y = writeIndex; y < levelData.rows; y++)
            {
                int randomColor = UnityEngine.Random.Range(0, levelData.colorCount);

                Vector2 spawnPos = new Vector2(x, levelData.rows + 1); // Above screen
                Vector2 targetPos = new Vector2(x, y);

                // Get from Pool
                Block newBlock = BlockPool.Instance.GetBlock();

                // Manually set transform since we are not using Instantiate directly here
                newBlock.transform.position = spawnPos;
                newBlock.transform.rotation = Quaternion.identity;
                newBlock.transform.SetParent(this.transform);

                newBlock.Init(x, y, randomColor, blockStyles[randomColor].iconDefault);
                allBlocks[x, y] = newBlock;

                newBlock.MoveToPosition(targetPos, 0.4f);
            }
        }

        yield return new WaitForSeconds(0.4f);
    }

    private List<Block> GetConnectedBlocks(Block startBlock)
    {
        List<Block> connectedBlocks = new List<Block>();
        int targetColor = startBlock.colorIndex;
        bool[,] visited = new bool[levelData.columns, levelData.rows];
        Queue<Block> queue = new Queue<Block>();
        queue.Enqueue(startBlock);
        visited[startBlock.x, startBlock.y] = true;

        while (queue.Count > 0)
        {
            Block current = queue.Dequeue();
            connectedBlocks.Add(current);
            CheckNeighbor(current.x + 1, current.y, targetColor, visited, queue);
            CheckNeighbor(current.x - 1, current.y, targetColor, visited, queue);
            CheckNeighbor(current.x, current.y + 1, targetColor, visited, queue);
            CheckNeighbor(current.x, current.y - 1, targetColor, visited, queue);
        }
        return connectedBlocks;
    }

    private void CheckNeighbor(int x, int y, int targetColor, bool[,] visited, Queue<Block> queue)
    {
        if (x < 0 || x >= levelData.columns || y < 0 || y >= levelData.rows) return;
        if (allBlocks[x, y] == null) return;
        if (visited[x, y]) return;
        if (allBlocks[x, y].colorIndex == targetColor)
        {
            visited[x, y] = true;
            queue.Enqueue(allBlocks[x, y]);
        }
    }

    private void RemoveBlock(Block block)
    {
        allBlocks[block.x, block.y] = null;
        block.PlayDestroyAnimation();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < levelData.columns; x++)
        {
            for (int y = 0; y < levelData.rows; y++)
            {
                int randomColor = UnityEngine.Random.Range(0, levelData.colorCount);

                Block newBlock = BlockPool.Instance.GetBlock();

                // Initial position is exact grid coordinate
                newBlock.transform.position = new Vector2(x, y);
                newBlock.transform.rotation = Quaternion.identity;
                newBlock.transform.SetParent(this.transform);

                newBlock.Init(x, y, randomColor, blockStyles[randomColor].iconDefault);
                allBlocks[x, y] = newBlock;

                newBlock.PlaySpawnAnimation();
            }
        }
    }

    private void AdjustCamera()
    {
        Camera.main.transform.position = new Vector3((float)levelData.columns / 2 - 0.5f, (float)levelData.rows / 2 - 0.5f, -10);
        float aspectRatio = (float)Screen.width / Screen.height;
        float verticalSize = levelData.rows / 2f + 1f;
        float horizontalSize = (levelData.columns / 2f + 1f) / aspectRatio;
        Camera.main.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }
}
