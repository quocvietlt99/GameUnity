using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    [Header("Score UI")]
    public TMP_Text currentScoreText;
    public TMP_Text startHighScoreText;
    public TMP_Text finalScoreText;
    public TMP_Text gameOverHighScoreText;

    private int currentScore = 0;

    // Điểm cao chỉ lưu trong phiên chạy game hiện tại.
    // Tắt game mở lại thì tự về 0.
    private static int sessionHighScore = 0;

    void Start()
    {
        currentScore = 0;

        UpdateCurrentScoreUI();
        UpdateHighScoreUI();
        UpdateFinalScoreUI();
    }

    public void AddScore()
    {
        currentScore++;
        UpdateCurrentScoreUI();
    }

    public void ShowFinalScoreOnly()
    {
        // Khi thua chỉ hiện điểm vừa chơi.
        // Chưa lưu điểm cao tại đây.
        UpdateFinalScoreUI();
        UpdateHighScoreUI();
    }

    public void SaveHighScoreWhenReplay()
    {
        // Chỉ khi bấm Chơi lại mới lưu điểm cao.
        if (currentScore > sessionHighScore)
        {
            sessionHighScore = currentScore;
        }
    }

    void UpdateCurrentScoreUI()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = currentScore.ToString();
        }
        else
        {
            Debug.LogError("Chưa gán CurrentScoreText vào Score script!");
        }
    }

    void UpdateHighScoreUI()
    {
        if (startHighScoreText != null)
        {
            startHighScoreText.text = "Điểm cao: " + sessionHighScore;
        }

        if (gameOverHighScoreText != null)
        {
            gameOverHighScoreText.text = "Điểm cao: " + sessionHighScore;
        }
    }

    void UpdateFinalScoreUI()
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = "Điểm: " + currentScore;
        }
    }
}