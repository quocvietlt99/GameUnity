using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject startPanel;
    public GameObject inGamePanel;
    public GameObject gameOverPanel;

    private bool gameStarted = false;
    private bool gameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;

        gameStarted = false;
        gameOver = false;

        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        if (inGamePanel != null)
        {
            inGamePanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        gameOver = false;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySwoosh();
        }

        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        if (inGamePanel != null)
        {
            inGamePanel.SetActive(true);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        BirdController bird = FindObjectOfType<BirdController>();

        if (bird != null)
        {
            bird.StartBird();
        }
    }

    public void GameOver()
    {
        if (gameOver) return;

        gameOver = true;
        gameStarted = false;

        Score score = FindObjectOfType<Score>();

        if (score != null)
        {
            score.ShowFinalScoreOnly();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayHit();
            SoundManager.Instance.PlayDie();
        }

        if (inGamePanel != null)
        {
            inGamePanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void Replay()
    {
        StartCoroutine(ReplayRoutine());
    }

    IEnumerator ReplayRoutine()
    {
        Score score = FindObjectOfType<Score>();

        if (score != null)
        {
            score.SaveHighScoreWhenReplay();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySwoosh();
        }

        // Dùng WaitForSecondsRealtime vì lúc Game Over Time.timeScale = 0
        yield return new WaitForSecondsRealtime(0.15f);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGameStarted()
    {
        return gameStarted;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }
}