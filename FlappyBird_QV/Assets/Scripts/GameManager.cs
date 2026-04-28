using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverImage;

    private int score;
    private bool isGameOver;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        score = 0;
        UpdateScoreUI();

        if (gameOverImage != null)
            gameOverImage.SetActive(false);
    }

    private void Update()
    {
        if (isGameOver && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void AddScore()
    {
        if (isGameOver) return;

        score++;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (gameOverImage != null)
            gameOverImage.SetActive(true);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}