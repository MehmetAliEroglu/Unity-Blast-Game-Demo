using UnityEngine;


public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private UIManager uiManager;

    [Header("Level Configuration")]
    [SerializeField] private LevelData currentLevel; // DRAG & DROP YOUR LEVEL HERE

    private int currentMoves;
    private int currentScore;
    private int targetScore;
    private bool isGameOver = false;

    private void Awake()
    {
        // Force 60 FPS for smooth gameplay on mobile devices
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (currentLevel == null)
        {
            Debug.LogError("Current Level is missing in GameManager!");
            return;
        }

        // Initialize Game State
        currentMoves = currentLevel.moveCount;
        targetScore = currentLevel.targetScore;
        currentScore = 0;
        isGameOver = false;

        // CRITICAL STEP: Pass the Level Data to the Board Manager
        boardManager.Initialize(currentLevel);

        // Update UI
        uiManager.UpdateMoves(currentMoves);
        uiManager.UpdateScore(currentScore, targetScore);

        // Subscribe to events
        boardManager.OnBlockBlasted += HandleBlockBlasted;
    }

    private void HandleBlockBlasted(int blastCount)
    {
        if (isGameOver) return;

        // Decrease Moves
        currentMoves--;
        uiManager.UpdateMoves(currentMoves);

        // Calculate Score (Exponential: n * n * 10)
        int earnedPoints = blastCount * blastCount * 10;
        currentScore += earnedPoints;

        uiManager.UpdateScore(currentScore, targetScore);

        CheckGameStatus();
    }

    private void CheckGameStatus()
    {
        // Win Condition
        if (currentScore >= targetScore)
        {
            FinishGame(true);
        }
        // Lose Condition
        else if (currentMoves <= 0)
        {
            FinishGame(false);
        }
    }

    private void FinishGame(bool isWin)
    {
        isGameOver = true;
        boardManager.inputLocked = true; // Lock the board
        uiManager.ShowGameOver(isWin);
    }

    private void OnDestroy()
    {
        if (boardManager != null)
            boardManager.OnBlockBlasted -= HandleBlockBlasted;
    }
}