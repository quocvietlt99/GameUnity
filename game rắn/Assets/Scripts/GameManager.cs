using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;

    private int score = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ResetGame();
    }

    public void AddScore()
    {
        if (isGameOver) return;

        score++;
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // 🔥 HÀM RESTART
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetGame()
    {
        score = 0;
        scoreText.text = "Score: 0";
        isGameOver = false;
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
