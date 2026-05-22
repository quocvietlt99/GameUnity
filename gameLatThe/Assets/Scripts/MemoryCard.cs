using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    [Header("Visual")]
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite frontBackgroundSprite;

    private int cardId;
    private Sprite frontSprite;
    private bool isFlipped;
    private bool isMatched;
    private MemoryGameManager gameManager;

    public int CardId => cardId;
    public bool IsFlipped => isFlipped;
    public bool IsMatched => isMatched;

    public void Initialize(
        int id,
        Sprite icon,
        Sprite cardBack,
        Sprite cardFrontBackground,
        MemoryGameManager manager)
    {
        cardId = id;
        frontSprite = icon;
        backSprite = cardBack;
        frontBackgroundSprite = cardFrontBackground;
        gameManager = manager;

        isFlipped = false;
        isMatched = false;

        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnCardClicked);

        ResetVisual();
        ShowBack();
    }

    private void OnCardClicked()
    {
        if (gameManager == null) return;
        gameManager.OnCardSelected(this);
    }

    public void FlipToFront()
    {
        if (isMatched) return;

        isFlipped = true;

        if (backgroundImage != null && frontBackgroundSprite != null)
        {
            backgroundImage.sprite = frontBackgroundSprite;
        }

        if (iconImage != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = frontSprite;
        }
    }

    public void FlipToBack()
    {
        if (isMatched) return;

        isFlipped = false;
        ShowBack();
    }

    public void SetMatched()
    {
        isMatched = true;
        isFlipped = true;

        if (button != null)
        {
            button.interactable = false;
        }

        if (backgroundImage != null)
        {
            Color color = backgroundImage.color;
            color.a = 0.65f;
            backgroundImage.color = color;
        }
    }

    public void ShowHint()
    {
        if (isMatched) return;
        FlipToFront();
    }

    public void HideHint()
    {
        if (isMatched) return;
        FlipToBack();
    }

    private void ShowBack()
    {
        if (backgroundImage != null && backSprite != null)
        {
            backgroundImage.sprite = backSprite;
        }

        if (iconImage != null)
        {
            iconImage.enabled = false;
            iconImage.sprite = null;
        }
    }

    private void ResetVisual()
    {
        if (button != null)
        {
            button.interactable = true;
        }

        if (backgroundImage != null)
        {
            Color color = backgroundImage.color;
            color.a = 1f;
            backgroundImage.color = color;
        }
    }
}