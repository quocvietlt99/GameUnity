using UnityEngine;

public class TetrisPreviewPanel : MonoBehaviour
{
    [Header("Preview Size")]
    [SerializeField] private float previewCellSize = 0.28f;
    [SerializeField] private float blockFill = 0.95f;

    [Header("Rendering")]
    [SerializeField] private int sortingOrder = 200;
    [SerializeField] private string sortingLayerName = "Default";

    [Header("Position Offset")]
    [SerializeField] private Vector3 localOffset = Vector3.zero;

    private GameObject previewRoot;

    public void Render(GameObject prefab, TetrominoKind kind)
    {
        Clear();

        Sprite sprite = GetSpriteFromPrefab(prefab);

        if (sprite == null)
        {
            Debug.LogWarning("Preview không tìm thấy Sprite trong prefab: " + (prefab != null ? prefab.name : "NULL"));
            return;
        }

        previewRoot = new GameObject("Preview_" + kind);
        previewRoot.transform.SetParent(transform);
        previewRoot.transform.localPosition = localOffset;
        previewRoot.transform.localRotation = Quaternion.identity;
        previewRoot.transform.localScale = Vector3.one;

        Vector2Int[] cells = GetPreviewCells(kind);
        Vector2 center = GetCenter(cells);

        for (int i = 0; i < cells.Length; i++)
        {
            GameObject block = new GameObject("PreviewBlock_" + i);
            block.transform.SetParent(previewRoot.transform);

            float x = (cells[i].x - center.x) * previewCellSize;
            float y = (cells[i].y - center.y) * previewCellSize;

            block.transform.localPosition = new Vector3(x, y, 0f);
            block.transform.localRotation = Quaternion.identity;

            SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = sortingOrder;

            FitSpriteToCell(renderer);
        }
    }

    public void Clear()
    {
        if (previewRoot != null)
        {
            Destroy(previewRoot);
            previewRoot = null;
        }
    }

    private Sprite GetSpriteFromPrefab(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        SpriteRenderer renderer = prefab.GetComponentInChildren<SpriteRenderer>();

        if (renderer == null)
        {
            return null;
        }

        return renderer.sprite;
    }

    private void FitSpriteToCell(SpriteRenderer renderer)
    {
        if (renderer == null || renderer.sprite == null)
        {
            return;
        }

        Vector2 spriteSize = renderer.sprite.bounds.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
        {
            return;
        }

        float targetSize = previewCellSize * blockFill;

        float scaleX = targetSize / spriteSize.x;
        float scaleY = targetSize / spriteSize.y;

        renderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    private Vector2 GetCenter(Vector2Int[] cells)
    {
        Vector2 sum = Vector2.zero;

        for (int i = 0; i < cells.Length; i++)
        {
            sum += cells[i];
        }

        return sum / cells.Length;
    }

    private Vector2Int[] GetPreviewCells(TetrominoKind kind)
    {
        switch (kind)
        {
            case TetrominoKind.I:
                return new[]
                {
                    new Vector2Int(-2, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0)
                };

            case TetrominoKind.O:
                return new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1)
                };

            case TetrominoKind.T:
                return new[]
                {
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1)
                };

            case TetrominoKind.S:
                return new[]
                {
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1)
                };

            case TetrominoKind.Z:
                return new[]
                {
                    new Vector2Int(-1, 1),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0)
                };

            case TetrominoKind.J:
                return new[]
                {
                    new Vector2Int(-1, 1),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0)
                };

            case TetrominoKind.L:
                return new[]
                {
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(1, 1)
                };

            default:
                return new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1)
                };
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(1.2f, 0.7f, 0f));
    }
}