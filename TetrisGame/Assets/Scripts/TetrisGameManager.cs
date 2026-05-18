using UnityEngine;
using UnityEngine.SceneManagement;

public class TetrisGameManager : MonoBehaviour
{
    public static TetrisGameManager Instance { get; private set; }

    private static int sessionHighScore = 0;

    [Header("Score Settings")]
    [SerializeField] private int lockedPieceScore = 10;
    [SerializeField] private int softDropScorePerCell = 1;
    [SerializeField] private int hardDropScorePerCell = 2;
    [SerializeField] private int comboBonus = 50;

    [Header("Line Clear Score")]
    [SerializeField] private int singleLineScore = 100;
    [SerializeField] private int doubleLineScore = 300;
    [SerializeField] private int tripleLineScore = 500;
    [SerializeField] private int tetrisLineScore = 800;

    [Header("T-Spin Score")]
    [SerializeField] private int tSpinNoLineScore = 400;
    [SerializeField] private int tSpinSingleScore = 800;
    [SerializeField] private int tSpinDoubleScore = 1200;
    [SerializeField] private int tSpinTripleScore = 1600;

    [Header("Level Settings")]
    [SerializeField] private float baseFallInterval = 0.8f;
    [SerializeField] private float minimumFallInterval = 0.08f;
    [SerializeField] private int linesPerLevel = 10;

    [Header("Debug")]
    [SerializeField] private bool enableDebugScoreKey = true;

    [Header("UI")]
    [SerializeField] private TetrisUI ui;

    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public int Level { get; private set; } = 1;
    public int Lines { get; private set; }
    public int Combo { get; private set; } = -1;
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;

        Score = 0;
        HighScore = sessionHighScore;
        Level = 1;
        Lines = 0;
        Combo = -1;
        IsGameOver = false;
    }

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        if (!enableDebugScoreKey)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            AddScore(100);
        }
    }

    public float GetFallInterval()
    {
        float interval = baseFallInterval * Mathf.Pow(0.85f, Level - 1);
        return Mathf.Max(minimumFallInterval, interval);
    }

    public void AddSoftDropScore()
    {
        if (IsGameOver)
        {
            return;
        }

        AddScore(softDropScorePerCell);
    }

    public void AddHardDropScore(int cells)
    {
        if (IsGameOver)
        {
            return;
        }

        AddScore(cells * hardDropScorePerCell);
    }

    public void OnPieceLocked(int clearedLines, bool isTSpin)
    {
        if (IsGameOver)
        {
            return;
        }

        AddScore(lockedPieceScore);

        if (clearedLines > 0)
        {
            Lines += clearedLines;
            Level = Mathf.Max(1, Lines / linesPerLevel + 1);
            Combo++;
        }
        else
        {
            Combo = -1;
        }

        int gainedScore = CalculateLineScore(clearedLines, isTSpin);

        if (gainedScore > 0)
        {
            AddScore(gainedScore * Level);
        }

        if (clearedLines > 0 && Combo > 0)
        {
            AddScore(Combo * comboBonus * Level);
        }

        RefreshUI();
    }

    private int CalculateLineScore(int clearedLines, bool isTSpin)
    {
        if (isTSpin)
        {
            if (clearedLines == 0) return tSpinNoLineScore;
            if (clearedLines == 1) return tSpinSingleScore;
            if (clearedLines == 2) return tSpinDoubleScore;
            return tSpinTripleScore;
        }

        if (clearedLines == 1) return singleLineScore;
        if (clearedLines == 2) return doubleLineScore;
        if (clearedLines == 3) return tripleLineScore;
        if (clearedLines >= 4) return tetrisLineScore;

        return 0;
    }

    public void AddScore(int amount)
    {
        if (IsGameOver)
        {
            return;
        }

        Score += Mathf.Max(0, amount);
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