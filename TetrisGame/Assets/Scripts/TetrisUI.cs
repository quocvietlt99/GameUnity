using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TetrisUI : MonoBehaviour
{
    [Header("Gameplay Text")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text linesText;
    [SerializeField] private TMP_Text comboText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverTitleText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text finalHighScoreText;
    [SerializeField] private Button replayButton;

    private void Awake()
    {
        if (replayButton != null)
        {
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(OnReplayButtonClicked);
        }

        SetGameOverPanel(false);
    }

    private void Start()
    {
        RefreshStats(0, 0, 1, 0, -1, false);
    }

    public void RefreshStats(
        int score,
        int highScore,
        int level,
        int lines,
        int combo,
        bool isGameOver
    )
    {
        if (scoreText != null)
        {
            scoreText.text = "Score\n" + score;
        }

        if (highScoreText != null)
        {
            highScoreText.text = "High Score\n" + highScore;
        }

        if (levelText != null)
        {
            levelText.text = "Level\n" + level;
        }

        if (linesText != null)
        {
            linesText.text = "Lines\n" + lines;
        }

        if (comboText != null)
        {
            comboText.text = combo > 0 ? "Combo\n" + combo : "Combo\n-";
        }

        if (isGameOver)
        {
            ShowGameOver(score, highScore);
        }
        else
        {
            SetGameOverPanel(false);
        }
    }

    private void ShowGameOver(int score, int highScore)
    {
        SetGameOverPanel(true);

        if (gameOverTitleText != null)
        {
            gameOverTitleText.text = "GAME OVER";
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score;
        }

        if (finalHighScoreText != null)
        {
            finalHighScoreText.text = "High Score: " + highScore;
        }
    }

    private void SetGameOverPanel(bool visible)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(visible);
        }
    }

    private void OnReplayButtonClicked()
    {
        if (TetrisGameManager.Instance != null)
        {
            TetrisGameManager.Instance.RestartGame();
        }
    }
}