using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class FitSpriteToCamera : MonoBehaviour
{
    public enum FitMode
    {
        Cover,
        Contain,
        Stretch
    }

    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private SpriteRenderer spriteRendererCache;

    [Header("Fit")]
    [SerializeField] private FitMode fitMode = FitMode.Cover;
    [SerializeField] private bool autoFit = true;

    [Header("Manual Tuning")]
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private float extraScale = 1.0f;
    [SerializeField] private float manualScaleMultiplier = 1.0f;
    [SerializeField] private float zPosition = 5f;

    [Header("Rendering")]
    [SerializeField] private int sortingOrder = -100;

    private void Awake()
    {
        Apply();
    }

    private void OnEnable()
    {
        Apply();
    }

    private void LateUpdate()
    {
        Apply();
    }

    private void OnValidate()
    {
        Apply();
    }

    public void Apply()
    {
        if (spriteRendererCache == null)
        {
            spriteRendererCache = GetComponent<SpriteRenderer>();
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (spriteRendererCache == null || spriteRendererCache.sprite == null || targetCamera == null)
        {
            return;
        }

        spriteRendererCache.sortingOrder = sortingOrder;

        transform.position = new Vector3(
            targetCamera.transform.position.x + offset.x,
            targetCamera.transform.position.y + offset.y,
            zPosition
        );

        if (!autoFit)
        {
            transform.localScale = Vector3.one * manualScaleMultiplier;
            return;
        }

        Bounds spriteBounds = spriteRendererCache.sprite.bounds;
        float spriteWidth = spriteBounds.size.x;
        float spriteHeight = spriteBounds.size.y;

        if (spriteWidth <= 0f || spriteHeight <= 0f)
        {
            return;
        }

        float camHeight = targetCamera.orthographicSize * 2f;
        float camWidth = camHeight * targetCamera.aspect;

        float scaleX = camWidth / spriteWidth;
        float scaleY = camHeight / spriteHeight;

        Vector3 finalScale = Vector3.one;

        switch (fitMode)
        {
            case FitMode.Cover:
                {
                    float uniform = Mathf.Max(scaleX, scaleY) * extraScale * manualScaleMultiplier;
                    finalScale = new Vector3(uniform, uniform, 1f);
                    break;
                }

            case FitMode.Contain:
                {
                    float uniform = Mathf.Min(scaleX, scaleY) * extraScale * manualScaleMultiplier;
                    finalScale = new Vector3(uniform, uniform, 1f);
                    break;
                }

            case FitMode.Stretch:
                {
                    finalScale = new Vector3(
                        scaleX * extraScale * manualScaleMultiplier,
                        scaleY * extraScale * manualScaleMultiplier,
                        1f
                    );
                    break;
                }
        }

        transform.localScale = finalScale;
    }
}