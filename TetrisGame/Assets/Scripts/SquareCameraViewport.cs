using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class SquareCameraViewport : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Square Settings")]
    [SerializeField] private bool forceOrthographic = true;
    [SerializeField] private float orthographicSize = 3.85f;
    [SerializeField] private bool centerViewport = true;

    [Header("Viewport Padding (0 -> 0.4)")]
    [SerializeField][Range(0f, 0.4f)] private float paddingLeft = 0f;
    [SerializeField][Range(0f, 0.4f)] private float paddingRight = 0f;
    [SerializeField][Range(0f, 0.4f)] private float paddingTop = 0f;
    [SerializeField][Range(0f, 0.4f)] private float paddingBottom = 0f;

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

    private void Apply()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        if (targetCamera == null)
        {
            return;
        }

        if (forceOrthographic)
        {
            targetCamera.orthographic = true;
            targetCamera.orthographicSize = orthographicSize;
        }

        float safeLeft = Mathf.Clamp01(paddingLeft);
        float safeRight = Mathf.Clamp01(paddingRight);
        float safeTop = Mathf.Clamp01(paddingTop);
        float safeBottom = Mathf.Clamp01(paddingBottom);

        float usableWidth = Mathf.Max(0.01f, 1f - safeLeft - safeRight);
        float usableHeight = Mathf.Max(0.01f, 1f - safeTop - safeBottom);

        float screenWidth = Mathf.Max(1f, Screen.width);
        float screenHeight = Mathf.Max(1f, Screen.height);

        float screenAspect = screenWidth / screenHeight;

        Rect rect;

        if (screenAspect >= 1f)
        {
            float squareWidthNormalized = usableHeight / screenAspect;

            float x = safeLeft;
            if (centerViewport)
            {
                x = safeLeft + (usableWidth - squareWidthNormalized) * 0.5f;
            }

            rect = new Rect(
                x,
                safeBottom,
                squareWidthNormalized,
                usableHeight
            );
        }
        else
        {
            float squareHeightNormalized = usableWidth * screenAspect;

            float y = safeBottom;
            if (centerViewport)
            {
                y = safeBottom + (usableHeight - squareHeightNormalized) * 0.5f;
            }

            rect = new Rect(
                safeLeft,
                y,
                usableWidth,
                squareHeightNormalized
            );
        }

        targetCamera.rect = rect;
    }
}