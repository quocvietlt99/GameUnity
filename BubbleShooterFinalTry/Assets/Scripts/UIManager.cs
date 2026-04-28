using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI shotsText;
    [SerializeField] private TextMeshProUGUI comboText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;

    public void Refresh(int score, int shots, int combo)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (shotsText != null)
            shotsText.text = "Balls: " + shots;

        if (comboText != null)
            comboText.text = "Combo: x" + combo;
    }

    public void HidePanels()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(int score)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverScoreText != null)
            gameOverScoreText.text = "Score: " + score;
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}