using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SimpleGameManager : MonoBehaviour
{
    public static SimpleGameManager Instance;

    [Header("Game State")]
    public bool isPlaying = false;
    public bool isGameOver = false;

    [Header("Speed")]
    public float worldSpeed = 5f;
    public float speedIncrease = 0.2f;
    public float maxSpeed = 12f;

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
        isPlaying = false;
        isGameOver = false;
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
        if (!isPlaying && !isGameOver)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }

            return;
        }

        if (!isPlaying || isGameOver)
            return;

        scoreCounter += Time.deltaTime * scoreSpeed;
        score = Mathf.FloorToInt(scoreCounter);

        worldSpeed += speedIncrease * Time.deltaTime;
        worldSpeed = Mathf.Clamp(worldSpeed, 0f, maxSpeed);

        UpdateScoreUI();
    }

    public void StartGame()
    {
        isPlaying = true;
        isGameOver = false;
        score = 0;
        scoreCounter = 0f;
        worldSpeed = 5f;

        if (startPanel != null)
            startPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateScoreUI();
    }

    public void GameOver()
    {
        isPlaying = false;
        isGameOver = true;

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