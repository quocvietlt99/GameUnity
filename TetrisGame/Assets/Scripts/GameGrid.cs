using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public const int Width = 10;
    public const int Height = 20;

    [Header("Fit To Middle Frame")]
    [SerializeField] private bool fitToMiddleFrame = true;

    [Tooltip("Tâm của ô giữa background")]
    [SerializeField] private Vector2 frameCenter = new Vector2(-0.05f, -0.30f);

    [Tooltip("Chiều rộng vùng chơi thực tế")]
    [SerializeField] private float frameWidth = 2.60f;

    [Tooltip("Chiều cao vùng chơi thực tế")]
    [SerializeField] private float frameHeight = 5.45f;

    [Tooltip("Khoảng chừa viền trong khung")]
    [SerializeField] private float framePadding = 0.06f;

    [Header("Manual Mode")]
    [SerializeField] private Vector2 manualOrigin = Vector2.zero;
    [SerializeField] private float manualStep = 0.25f;

    public static float Step { get; private set; } = 0.25f;
    public static Vector2 Origin { get; private set; }
    public static Transform[,] Grid { get; private set; } = new Transform[Width, Height];

    private void Awake()
    {
        SetupGridTransform();
        ClearGrid();
    }

    private void SetupGridTransform()
    {
        if (!fitToMiddleFrame)
        {
            Step = manualStep;
            Origin = manualOrigin;
            return;
        }

        float usableWidth = Mathf.Max(0.1f, frameWidth - framePadding * 2f);
        float usableHeight = Mathf.Max(0.1f, frameHeight - framePadding * 2f);

        float stepByWidth = usableWidth / Width;
        float stepByHeight = usableHeight / Height;

        Step = Mathf.Min(stepByWidth, stepByHeight);

        float boardWidth = Width * Step;
        float boardHeight = Height * Step;

        float originX = frameCenter.x - boardWidth * 0.5f + Step * 0.5f;
        float originY = frameCenter.y - boardHeight * 0.5f + Step * 0.5f;

        Origin = new Vector2(originX, originY);
    }

    public static void ClearGrid()
    {
        Grid = new Transform[Width, Height];
    }

    public static Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - Origin.x) / Step);
        int y = Mathf.RoundToInt((worldPosition.y - Origin.y) / Step);

        return new Vector2Int(x, y);
    }

    public static Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(
            Origin.x + x * Step,
            Origin.y + y * Step,
            0f
        );
    }

    public static bool IsInsideHorizontal(int x)
    {
        return x >= 0 && x < Width;
    }

    public static bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public static bool IsOccupied(int x, int y)
    {
        if (!IsInsideGrid(x, y))
        {
            return false;
        }

        return Grid[x, y] != null;
    }

    public static int ClearFullLines()
    {
        int clearedLines = 0;

        for (int y = 0; y < Height; y++)
        {
            if (IsLineFull(y))
            {
                DeleteLine(y);
                MoveRowsDownAbove(y);
                clearedLines++;
                y--;
            }
        }

        return clearedLines;
    }

    private static bool IsLineFull(int y)
    {
        int count = 0;

        for (int x = 0; x < Width; x++)
        {
            if (Grid[x, y] != null)
            {
                count++;
            }
        }

        return count == Width;
    }

    private static void DeleteLine(int y)
    {
        for (int x = 0; x < Width; x++)
        {
            if (Grid[x, y] != null)
            {
                Object.Destroy(Grid[x, y].gameObject);
                Grid[x, y] = null;
            }
        }
    }

    private static void MoveRowsDownAbove(int deletedRow)
    {
        for (int y = deletedRow + 1; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Grid[x, y] == null)
                {
                    continue;
                }

                Grid[x, y - 1] = Grid[x, y];
                Grid[x, y] = null;

                Grid[x, y - 1].position = GridToWorld(x, y - 1);
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawPreview();
    }

    private void DrawPreview()
    {
        float previewStep;
        Vector2 previewOrigin;

        if (fitToMiddleFrame)
        {
            float usableWidth = Mathf.Max(0.1f, frameWidth - framePadding * 2f);
            float usableHeight = Mathf.Max(0.1f, frameHeight - framePadding * 2f);

            float stepByWidth = usableWidth / Width;
            float stepByHeight = usableHeight / Height;

            previewStep = Mathf.Min(stepByWidth, stepByHeight);

            float boardWidth = Width * previewStep;
            float boardHeight = Height * previewStep;

            previewOrigin = new Vector2(
                frameCenter.x - boardWidth * 0.5f + previewStep * 0.5f,
                frameCenter.y - boardHeight * 0.5f + previewStep * 0.5f
            );
        }
        else
        {
            previewStep = manualStep;
            previewOrigin = manualOrigin;
        }

        Gizmos.color = Color.green;

        for (int x = 0; x <= Width; x++)
        {
            Vector3 from = new Vector3(
                previewOrigin.x - previewStep * 0.5f + x * previewStep,
                previewOrigin.y - previewStep * 0.5f,
                0f
            );

            Vector3 to = new Vector3(
                previewOrigin.x - previewStep * 0.5f + x * previewStep,
                previewOrigin.y - previewStep * 0.5f + Height * previewStep,
                0f
            );

            Gizmos.DrawLine(from, to);
        }

        for (int y = 0; y <= Height; y++)
        {
            Vector3 from = new Vector3(
                previewOrigin.x - previewStep * 0.5f,
                previewOrigin.y - previewStep * 0.5f + y * previewStep,
                0f
            );

            Vector3 to = new Vector3(
                previewOrigin.x - previewStep * 0.5f + Width * previewStep,
                previewOrigin.y - previewStep * 0.5f + y * previewStep,
                0f
            );

            Gizmos.DrawLine(from, to);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(frameCenter, new Vector3(frameWidth, frameHeight, 0f));
    }
}