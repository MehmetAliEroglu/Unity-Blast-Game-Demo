using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "BlastGame/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Board Settings")]
    public int rows = 9;
    public int columns = 7;
    public int colorCount = 5;

    [Header("Icon Thresholds ")]
    public int thresholdA = 4; // Group size > 4
    public int thresholdB = 7; // Group size > 7
    public int thresholdC = 9; // Group size > 9

    [Header("Game Rules")]
    public int moveCount = 20;
    public int targetScore = 1000;
}
