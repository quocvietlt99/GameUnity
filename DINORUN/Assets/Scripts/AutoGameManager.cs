using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoGameManager : MonoBehaviour
{
    public static AutoGameManager Instance;

    [Header("Game State")]
    public bool isPlaying = true;
    public bool isGameOver = false;

    [Header("Speed")]
    public float worldSpeed = 2.8f;
    public float startSpeed = 2.8f;
    public float speedIncrease = 0.03f;
    public float maxSpeed = 6f;

    [Header("Score")]
    public int score;
    public float scoreSpeed = 8f;

    private float scoreCounter;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        isPlaying = true;
        isGameOver = false;

        worldSpeed = startSpeed;
        score = 0;
        scoreCounter = 0f;

        Debug.Log("AutoGameManager started.");
    }

    private void Update()
    {
        if (!isPlaying || isGameOver)
            return;

        scoreCounter += Time.deltaTime * scoreSpeed;
        score = Mathf.FloorToInt(scoreCounter);

        worldSpeed += speedIncrease * Time.deltaTime;
        worldSpeed = Mathf.Clamp(worldSpeed, startSpeed, maxSpeed);
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isPlaying = false;
        isGameOver = true;

        Debug.Log("GAME OVER. Score = " + score);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}