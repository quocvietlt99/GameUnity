using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI scoreText;
    private int score = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        UpdateScore();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }
}
