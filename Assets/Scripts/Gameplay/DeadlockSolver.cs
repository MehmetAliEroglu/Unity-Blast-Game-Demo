using UnityEngine;
using System.Collections.Generic;

public static class DeadlockSolver
{
    /// <summary>
    /// Checks if there are any valid moves on the grid.
    /// Iterates through the grid and checks neighbors to the Right and Up to avoid double checking.
    /// </summary>
    /// <returns>True if no matches exist (Deadlock), False otherwise.</returns>
    public static bool IsDeadlocked(Block[,] grid, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Skip empty spaces
                if (grid[x, y] == null) continue;

                int currentColor = grid[x, y].colorIndex;

                // 1. Check Right Neighbor
                if (x < width - 1)
                {
                    Block rightBlock = grid[x + 1, y];
                    if (rightBlock != null && rightBlock.colorIndex == currentColor)
                        return false; // Match found, game can continue.
                }

                // 2. Check Upper Neighbor
                if (y < height - 1)
                {
                    Block upBlock = grid[x, y + 1];
                    if (upBlock != null && upBlock.colorIndex == currentColor)
                        return false; // Match found, game can continue.
                }
            }
        }
        return true; // No matches found.
    }

    /// <summary>
    /// Shuffles the board intelligently to guarantee a solution.
    /// Instead of random swapping, it collects all colors, shuffles them using Fisher-Yates,
    /// and redistributes them. If that fails, it forces a match.
    /// </summary>
    public static void SolveDeadlock(Block[,] grid, int width, int height, LevelData levelData)
    {
        Debug.Log("Deadlock Detected! Applying Smart Shuffle...");

        // 1. Collect all active blocks and their colors
        List<int> colorPool = new List<int>();
        List<Block> activeBlocks = new List<Block>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null)
                {
                    colorPool.Add(grid[x, y].colorIndex);
                    activeBlocks.Add(grid[x, y]);
                }
            }
        }

        // 2. Shuffle the color pool (Fisher-Yates Algorithm)
        ShuffleList(colorPool);

        // 3. Redistribute colors back to blocks
        for (int i = 0; i < activeBlocks.Count; i++)
        {
            // We pass 'null' for the sprite because BoardManager will call UpdateBoardVisuals() immediately after.
            activeBlocks[i].Init(activeBlocks[i].x, activeBlocks[i].y, colorPool[i], null);
        }

        // 4. GUARANTEE CHECK
        // If the shuffle accidentally resulted in another deadlock (rare but possible), force a match.
        if (IsDeadlocked(grid, width, height))
        {
            Debug.Log("Shuffle failed to create match. Forcing a match...");
            ForceCreateMatch(grid, width, height);
        }
    }

    /// <summary>
    /// Generic Fisher-Yates shuffle implementation.
    /// </summary>
    private static void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Finds the first available adjacent pair and forces them to be the same color.
    /// This ensures the game never gets stuck in an infinite deadlock loop.
    /// </summary>
    private static void ForceCreateMatch(Block[,] grid, int width, int height)
    {
        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Block current = grid[x, y];
                Block neighbor = grid[x + 1, y]; // Check right neighbor

                // Find two valid adjacent blocks
                if (current != null && neighbor != null)
                {
                    // Make the neighbor the same color as the current block
                    neighbor.Init(neighbor.x, neighbor.y, current.colorIndex, null);

                    Debug.Log($"Match forced at [{x},{y}] and [{x + 1},{y}]");
                    return; // Job done, exit immediately.
                }
            }
        }
    }
}
