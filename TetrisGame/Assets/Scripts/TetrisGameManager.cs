using UnityEngine;
using UnityEngine.SceneManagement;

public class TetrisGameManager : MonoBehaviour
{
    public static TetrisGameManager Instance { get; private set; }

    private static int sessionHighScore = 0;

    [Header("Score Settings")]
    [SerializeField] private int scorePerLine = 100;

    [Header("Level Settings")]
    [SerializeField] private float baseFallInterval = 0.8f;
    [SerializeField] private float minimumFallInterval = 0.08f;
    [SerializeField] private int linesPerLevel = 10;

    [Header("UI")]
    [SerializeField] private TetrisUI ui;

    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public int Level { get; private set; } = 1;
    public int Lines { get; private set; }

    // Combo ở đây chính là số hàng xóa cùng lúc.
    // Ví dụ xóa 3 hàng cùng lúc => Combo = 3.
    public int Combo { get; private set; }

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;

        Score = 0;
        HighScore = sessionHighScore;
        Level = 1;
        Lines = 0;
        Combo = 0;
        IsGameOver = false;
    }

    private void Start()
    {
        RefreshUI();
    }

    public float GetFallInterval()
    {
        float interval = baseFallInterval * Mathf.Pow(0.85f, Level - 1);
        return Mathf.Max(minimumFallInterval, interval);
    }

    // Không cộng điểm khi soft drop.
    public void AddSoftDropScore()
    {
        // Để trống theo yêu cầu: chỉ xóa hàng mới tính điểm.
    }

    // Không cộng điểm khi hard drop.
    public void AddHardDropScore(int cells)
    {
        // Để trống theo yêu cầu: chỉ xóa hàng mới tính điểm.
    }

    public void OnPieceLocked(int clearedLines, bool isTSpin)
    {
        if (IsGameOver)
        {
            return;
        }

        if (clearedLines <= 0)
        {
            Combo = 0;
            RefreshUI();
            return;
        }

        Lines += clearedLines;

        Level = Mathf.Max(1, Lines / linesPerLevel + 1);

        // Combo = số hàng xóa cùng lúc.
        // Ví dụ clearedLines = 3 thì UI sẽ hiện Combo 3.
        Combo = clearedLines;

        int gainedScore = clearedLines * scorePerLine * Level;

        Score += gainedScore;

        RefreshUI();
    }

    public void GameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;

        if (TetrisAudioManager.Instance != null)
        {
            TetrisAudioManager.Instance.PlayGameOver();
        }

        RefreshUI();
    }

    public void Restart()
    {
        RestartGame();
    }

    public void RestartGame()
    {
        CommitHighScoreOnlyWhenReplay();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CommitHighScoreOnlyWhenReplay()
    {
        if (Score > sessionHighScore)
        {
            sessionHighScore = Score;
        }
    }

    public void ResetHighScore()
    {
        sessionHighScore = 0;
        HighScore = 0;
        RefreshUI();
    }

    private void RefreshUI()
    {
        HighScore = sessionHighScore;

        if (ui != null)
        {
            ui.RefreshStats(
                Score,
                HighScore,
                Level,
                Lines,
                Combo,
                IsGameOver
            );
        }
    }
}