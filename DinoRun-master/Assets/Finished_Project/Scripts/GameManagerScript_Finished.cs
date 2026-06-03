using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript_Finished : MonoBehaviour
{
    public static GameManagerScript_Finished instance;

    [Header("Game State")]
    public bool gameRunning;

    [Header("Score")]
    public Text scoreText;
    public GameObject startText;
    public GameObject gameOverText;

    [Header("Score Settings")]
    public float scoreIncreaseSpeed = 10f;

    [Header("Speed Increase Every 300 Score")]
    public int scorePerSpeedIncrease = 300;
    public float speedIncreaseAmount = 0.15f;
    public float maxSpeedMultiplier = 2.5f;

    private float score;
    private int currentSpeedLevel;
    private bool isGameOver;

    public float speedMultiplier = 1f;

    void Start()
    {
        if (GameManagerScript_Finished.instance == null)
        {
            GameManagerScript_Finished.instance = this;
        }

        Time.timeScale = 1f;

        gameRunning = false;
        isGameOver = false;

        score = 0f;
        currentSpeedLevel = 0;
        speedMultiplier = 1f;

        if (startText != null)
        {
            startText.SetActive(true);
        }

        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        UpdateScoreText();
    }

    void Update()
    {
        if (isGameOver)
        {
            HandleRestartInput();
            return;
        }

        if (!gameRunning)
        {
            HandleStartInput();
            return;
        }

        UpdateScore();
        UpdateSpeedByScore();
    }

    private void HandleStartInput()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    private void HandleRestartInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetMouseButtonDown(0))
        {
            RestartGame();
        }
    }

    private void StartGame()
    {
        gameRunning = true;
        isGameOver = false;

        if (startText != null)
        {
            startText.SetActive(false);
        }

        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }
    }

    private void UpdateScore()
    {
        score += Time.deltaTime * scoreIncreaseSpeed;
        UpdateScoreText();
    }

    private void UpdateSpeedByScore()
    {
        int newSpeedLevel = Mathf.FloorToInt(score / scorePerSpeedIncrease);

        if (newSpeedLevel > currentSpeedLevel)
        {
            currentSpeedLevel = newSpeedLevel;

            speedMultiplier = 1f + currentSpeedLevel * speedIncreaseAmount;
            speedMultiplier = Mathf.Clamp(speedMultiplier, 1f, maxSpeedMultiplier);

            Debug.Log("Tang toc tai moc diem: " + currentSpeedLevel * scorePerSpeedIncrease + " | Speed Multiplier: " + speedMultiplier);
        }
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        gameRunning = false;

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        Time.timeScale = 0f;

        Debug.Log("GAME OVER");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = ((int)score).ToString("00000");
        }
    }

    public int GetScore()
    {
        return Mathf.FloorToInt(score);
    }

    public float GetSpeedMultiplier()
    {
        return speedMultiplier;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}