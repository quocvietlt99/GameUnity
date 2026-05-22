using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] private LevelData[] levels;

    [Header("Card Data")]
    [SerializeField] private Sprite[] cardSprites;
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private Sprite cardFrontBackgroundSprite;

    [Header("Prefab & Board")]
    [SerializeField] private MemoryCard cardPrefab;
    [SerializeField] private Transform boardParent;
    [SerializeField] private GridLayoutGroup boardGrid;

    [Header("Top UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private Button hintButton;
    [SerializeField] private TMP_Text hintButtonText;

    [Header("Win UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text finalMovesText;
    [SerializeField] private Button nextLevelButton;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip flipSound;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip hintSound;

    private LevelData currentLevel;
    private readonly List<MemoryCard> cards = new List<MemoryCard>();

    private MemoryCard firstCard;
    private MemoryCard secondCard;

    private bool isChecking;
    private bool isGameOver;
    private bool hintAvailable = true;

    private float currentTime;
    private float hintCooldownTimer;

    private int moves;
    private int score;
    private int combo;
    private int matchedPairs;
    private int selectedLevelIndex;

    private void Start()
    {
        AudioListener.volume = GameSettings.SoundEnabled ? 1f : 0f;

        selectedLevelIndex = Mathf.Clamp(GameSettings.SelectedLevelIndex, 0, levels.Length - 1);
        currentLevel = levels[selectedLevelIndex];

        ApplyDifficultyModifier();
        SetupButtonEvents();
        StartGame();
    }

    private void Update()
    {
        if (isGameOver) return;

        UpdateTimer();
        UpdateHintCooldown();
    }

    private void ApplyDifficultyModifier()
    {
        int difficulty = GameSettings.Difficulty;

        if (difficulty == 0)
        {
            currentTime = currentLevel.timeLimit * 1.25f;
        }
        else if (difficulty == 1)
        {
            currentTime = currentLevel.timeLimit;
        }
        else
        {
            currentTime = currentLevel.timeLimit * 0.8f;
        }
    }

    private void SetupButtonEvents()
    {
        if (hintButton != null)
        {
            hintButton.onClick.RemoveAllListeners();
            hintButton.onClick.AddListener(UseHint);
        }
    }

    private void StartGame()
    {
        if (currentLevel == null)
        {
            Debug.LogError("Không tìm thấy LevelData.");
            return;
        }

        if ((currentLevel.rows * currentLevel.columns) % 2 != 0)
        {
            Debug.LogError("Tổng số thẻ phải là số chẵn.");
            return;
        }

        if (cardSprites.Length < currentLevel.TotalPairs)
        {
            Debug.LogError("Không đủ sprite thẻ. Level này cần ít nhất " + currentLevel.TotalPairs + " ảnh khác nhau.");
            return;
        }

        isGameOver = false;
        isChecking = false;
        hintAvailable = true;

        moves = 0;
        score = 0;
        combo = 0;
        matchedPairs = 0;
        firstCard = null;
        secondCard = null;

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        ClearBoard();
        SetupBoardGrid();
        GenerateCards();
        UpdateUI();
    }

    private void ClearBoard()
    {
        cards.Clear();

        foreach (Transform child in boardParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetupBoardGrid()
    {
        if (boardGrid == null) return;

        boardGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        boardGrid.constraintCount = currentLevel.columns;

        RectTransform boardRect = boardParent.GetComponent<RectTransform>();

        if (boardRect == null) return;

        float boardWidth = boardRect.rect.width;
        float boardHeight = boardRect.rect.height;

        float spacingX = boardGrid.spacing.x;
        float spacingY = boardGrid.spacing.y;

        float totalSpacingX = spacingX * (currentLevel.columns - 1);
        float totalSpacingY = spacingY * (currentLevel.rows - 1);

        float cellWidth = (boardWidth - totalSpacingX - boardGrid.padding.left - boardGrid.padding.right) / currentLevel.columns;
        float cellHeight = (boardHeight - totalSpacingY - boardGrid.padding.top - boardGrid.padding.bottom) / currentLevel.rows;

        float cellSize = Mathf.Min(cellWidth, cellHeight);

        boardGrid.cellSize = new Vector2(cellSize, cellSize);
    }

    private void GenerateCards()
    {
        List<int> ids = new List<int>();

        for (int i = 0; i < currentLevel.TotalPairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        Shuffle(ids);

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];

            MemoryCard newCard = Instantiate(cardPrefab, boardParent);

            newCard.Initialize(
                id,
                cardSprites[id],
                cardBackSprite,
                cardFrontBackgroundSprite,
                this
            );

            cards.Add(newCard);
        }
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void OnCardSelected(MemoryCard selectedCard)
    {
        if (isChecking || isGameOver) return;
        if (selectedCard == null) return;
        if (selectedCard.IsMatched || selectedCard.IsFlipped) return;

        selectedCard.FlipToFront();
        PlaySound(flipSound);

        if (firstCard == null)
        {
            firstCard = selectedCard;
            return;
        }

        secondCard = selectedCard;
        moves++;

        StartCoroutine(CheckMatchRoutine());
    }

    private IEnumerator CheckMatchRoutine()
    {
        isChecking = true;

        yield return new WaitForSeconds(0.6f);

        if (firstCard != null && secondCard != null)
        {
            if (firstCard.CardId == secondCard.CardId)
            {
                HandleMatchSuccess();
            }
            else
            {
                HandleMatchFail();
            }
        }

        firstCard = null;
        secondCard = null;
        isChecking = false;

        UpdateUI();
    }

    private void HandleMatchSuccess()
    {
        firstCard.SetMatched();
        secondCard.SetMatched();

        matchedPairs++;
        combo++;

        int timeBonus = Mathf.Max(0, Mathf.RoundToInt(currentTime));
        int comboBonus = combo * 25;
        int movePenalty = moves * 2;

        int addScore = currentLevel.baseScorePerMatch + timeBonus + comboBonus - movePenalty;
        addScore = Mathf.Max(10, addScore);

        score += addScore;

        PlaySound(matchSound);

        if (matchedPairs >= currentLevel.TotalPairs)
        {
            WinGame();
        }
    }

    private void HandleMatchFail()
    {
        firstCard.FlipToBack();
        secondCard.FlipToBack();

        combo = 0;

        PlaySound(wrongSound);
    }

    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            LoseGame();
        }

        UpdateUI();
    }

    private void UpdateHintCooldown()
    {
        if (hintAvailable) return;

        hintCooldownTimer -= Time.deltaTime;

        if (hintCooldownTimer <= 0)
        {
            hintAvailable = true;
            hintCooldownTimer = 0;
        }

        UpdateHintUI();
    }

    private void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
        }

        if (movesText != null)
        {
            movesText.text = $"Moves: {moves}";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        if (comboText != null)
        {
            comboText.text = $"Combo: {combo}";
        }

        UpdateHintUI();
    }

    private void UpdateHintUI()
    {
        if (hintButton != null)
        {
            hintButton.interactable = hintAvailable && !isGameOver && !isChecking;
        }

        if (hintButtonText != null)
        {
            if (hintAvailable)
            {
                hintButtonText.text = "Hint";
            }
            else
            {
                hintButtonText.text = $"Hint {Mathf.CeilToInt(hintCooldownTimer)}s";
            }
        }
    }

    private void UseHint()
    {
        if (!hintAvailable || isGameOver || isChecking) return;

        MemoryCard cardA = null;
        MemoryCard cardB = null;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].IsMatched) continue;

            for (int j = i + 1; j < cards.Count; j++)
            {
                if (cards[j].IsMatched) continue;

                if (cards[i].CardId == cards[j].CardId)
                {
                    cardA = cards[i];
                    cardB = cards[j];
                    break;
                }
            }

            if (cardA != null && cardB != null) break;
        }

        if (cardA == null || cardB == null) return;

        StartCoroutine(HintRoutine(cardA, cardB));
    }

    private IEnumerator HintRoutine(MemoryCard cardA, MemoryCard cardB)
    {
        isChecking = true;
        hintAvailable = false;
        hintCooldownTimer = currentLevel.hintCooldown;

        PlaySound(hintSound);

        cardA.ShowHint();
        cardB.ShowHint();

        yield return new WaitForSeconds(1.2f);

        cardA.HideHint();
        cardB.HideHint();

        isChecking = false;
        UpdateUI();
    }

    private void WinGame()
    {
        isGameOver = true;

        int finalTimeBonus = Mathf.RoundToInt(currentTime * 5f);
        int finalMoveBonus = Mathf.Max(0, 500 - moves * 10);

        score += finalTimeBonus + finalMoveBonus;

        PlaySound(winSound);

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Score: {score}";
        }

        if (finalTimeText != null)
        {
            finalTimeText.text = $"Time Left: {Mathf.CeilToInt(currentTime)}";
        }

        if (finalMovesText != null)
        {
            finalMovesText.text = $"Moves: {moves}";
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.interactable = selectedLevelIndex < levels.Length - 1;
        }

        SaveHighScore();
        UpdateUI();
    }

    private void LoseGame()
    {
        isGameOver = true;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Time Out!\nScore: {score}";
        }

        if (finalTimeText != null)
        {
            finalTimeText.text = "Time Left: 0";
        }

        if (finalMovesText != null)
        {
            finalMovesText.text = $"Moves: {moves}";
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.interactable = false;
        }

        UpdateUI();
    }

    private void SaveHighScore()
    {
        string key = $"HighScore_Level_{selectedLevelIndex}";
        int oldHighScore = PlayerPrefs.GetInt(key, 0);

        if (score > oldHighScore)
        {
            PlayerPrefs.SetInt(key, score);
            PlayerPrefs.Save();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (!GameSettings.SoundEnabled) return;
        if (audioSource == null || clip == null) return;

        audioSource.PlayOneShot(clip);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NextLevel()
    {
        if (selectedLevelIndex >= levels.Length - 1) return;

        GameSettings.SelectedLevelIndex = selectedLevelIndex + 1;
        SceneManager.LoadScene("GameScene");
    }
}