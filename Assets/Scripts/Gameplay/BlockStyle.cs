using UnityEngine;

// [System.Serializable] is required to display this struct inside the Unity Inspector
[System.Serializable]
public struct BlockStyle
{
    public string name;          // Descriptive name (e.g., "Red", "Blue") for easier debugging
    public Sprite iconDefault;   // Standard sprite (No group)
    public Sprite iconA;         // Sprite for the first threshold (Small Group)
    public Sprite iconB;         // Sprite for the second threshold (Medium Group)
    public Sprite iconC;         // Sprite for the third threshold (Large Group)
}
