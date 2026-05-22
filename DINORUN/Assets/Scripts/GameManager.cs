using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPlaying { get; private set; }
    public bool IsGameOver { get; private set; }

    [Header("Score")]
    public int score;
    public float scoreSpeed = 10f;

    [Header("UI")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;

    private float scoreCounter;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        IsPlaying = false;
        IsGameOver = false;
        score = 0;
        scoreCounter = 0f;

        if (startPanel != null)
            startPanel.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateScoreUI();
    }

    private void Update()
    {
        if (!IsPlaying && !IsGameOver)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }

            return;
        }

        if (!IsPlaying || IsGameOver)
            return;

        scoreCounter += Time.deltaTime * scoreSpeed;
        score = Mathf.FloorToInt(scoreCounter);

        UpdateScoreUI();
    }

    public void StartGame()
    {
        IsPlaying = true;
        IsGameOver = false;
        score = 0;
        scoreCounter = 0f;

        if (startPanel != null)
            startPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateScoreUI();
    }

    public void GameOver()
    {
        IsPlaying = false;
        IsGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Điểm: " + score;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}