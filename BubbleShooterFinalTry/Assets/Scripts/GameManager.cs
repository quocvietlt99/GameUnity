using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ShooterController shooterController;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GridManager gridManager;

    [Header("Gameplay")]
    [SerializeField] private int startShots = 30;
    [SerializeField] private int startColorCount = 4;
    [SerializeField] private float loseY = -4.2f;

    private int score;
    private int combo;
    private int shotsLeft;
    private bool isEndGame;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        score = 0;
        combo = 1;
        shotsLeft = startShots;
        isEndGame = false;

        if (gridManager == null)
        {
            Debug.LogError("GameManager thiếu GridManager");
            return;
        }

        if (shooterController == null)
        {
            Debug.LogError("GameManager thiếu ShooterController");
            return;
        }

        if (uiManager == null)
        {
            Debug.LogError("GameManager thiếu UIManager");
            return;
        }

        gridManager.GenerateRandomGrid();
        shooterController.Init(startColorCount);
        uiManager.HidePanels();
        uiManager.Refresh(score, shotsLeft, combo);
    }

    public void UseShot()
    {
        if (isEndGame) return;

        shotsLeft--;
        uiManager.Refresh(score, shotsLeft, combo);

        if (shotsLeft <= 0)
        {
            GameOver();
        }
    }

    public void NotifyShotResolved(bool exploded, int poppedCount, int droppedCount)
    {
        if (isEndGame) return;

        if (exploded)
        {
            int gained = (poppedCount * 10 + droppedCount * 20) * combo;
            score += gained;
            combo++;
        }
        else
        {
            combo = 1;
        }

        uiManager.Refresh(score, shotsLeft, combo);
        shooterController.ReloadAfterShot();
    }

    public void CheckLoseLine()
    {
        if (isEndGame) return;
        if (gridManager == null) return;

        if (gridManager.HasBubbleBelow(loseY))
        {
            GameOver();
        }
    }

    public void Win()
    {
        if (isEndGame) return;
        isEndGame = true;

        // Tạm thời khi thắng chỉ log, nếu muốn mình sẽ thêm Victory UI sau
        Debug.Log("Victory! Score = " + score);
    }

    public void GameOver()
    {
        if (isEndGame) return;

        isEndGame = true;
        uiManager.ShowGameOver(score);
    }
}