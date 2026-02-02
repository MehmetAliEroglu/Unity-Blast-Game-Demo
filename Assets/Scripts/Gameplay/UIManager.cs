using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("In-Game UI")]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider scoreSlider; // Optional progress bar for better feedback

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject restartButtonObj;

    public void UpdateMoves(int moves)
    {
        movesText.text = moves.ToString();

        // Visual Feedback: Warn player by changing color when moves are critical (<= 5)
        if (moves <= 5) movesText.color = Color.red;
        else movesText.color = Color.white;
    }

    public void UpdateScore(int currentScore, int targetScore)
    {
        scoreText.text = $"{currentScore} / {targetScore}";

        if (scoreSlider != null)
        {
            scoreSlider.maxValue = targetScore;
            scoreSlider.value = currentScore;
        }
    }

    public void ShowGameOver(bool isWin)
    {
        gameOverPanel.SetActive(true);

        if (isWin)
        {
            resultText.text = "LEVEL COMPLETED";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "OUT OF MOVES";
            resultText.color = Color.red;
        }
    }

    // This function is linked to the Restart Button's OnClick event
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}