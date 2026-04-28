using UnityEngine;

public enum BubbleColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Purple
}

[RequireComponent(typeof(SpriteRenderer))]
public class Bubble : MonoBehaviour
{
    public BubbleColor colorId;
    public bool isAttached;

    [SerializeField] private GameObject popEffectPrefab;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetColor(BubbleColor color)
    {
        colorId = color;

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        sr.color = GetUnityColor(color);
    }

    public static Color GetUnityColor(BubbleColor color)
    {
        switch (color)
        {
            case BubbleColor.Red: return Color.red;
            case BubbleColor.Green: return Color.green;
            case BubbleColor.Blue: return Color.blue;
            case BubbleColor.Yellow: return Color.yellow;
            case BubbleColor.Purple: return new Color(0.7f, 0.3f, 1f);
            default: return Color.white;
        }
    }

    public void Pop()
    {
        if (popEffectPrefab != null)
        {
            Instantiate(popEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}