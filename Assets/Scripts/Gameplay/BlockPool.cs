using UnityEngine;
using System.Collections.Generic;

public class BlockPool : MonoBehaviour
{
    public static BlockPool Instance;

    [Header("Pool Settings")]
    [SerializeField] private Block blockPrefab;
    [SerializeField] private int initialPoolSize = 50; // Default size

    private Queue<Block> poolQueue = new Queue<Block>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        // FIX: Used 'initialPoolSize' instead of hardcoded '50'
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewBlock();
        }
    }

    public Block GetBlock()
    {
        // Check if there is an available block in the pool
        if (poolQueue.Count > 0)
        {
            Block block = poolQueue.Dequeue();
            block.gameObject.SetActive(true);
            return block;
        }
        else
        {
            // Pool is empty, instantiate a new one dynamically
            return Instantiate(blockPrefab, transform);
        }
    }

    public void ReturnBlock(Block block)
    {
        block.gameObject.SetActive(false); // Deactivate object
        poolQueue.Enqueue(block);          // Add back to queue
    }

    // Helper function to keep code clean
    private void CreateNewBlock()
    {
        Block newBlock = Instantiate(blockPrefab, transform);
        newBlock.gameObject.SetActive(false);
        poolQueue.Enqueue(newBlock);
    }
}