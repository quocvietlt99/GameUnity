using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private Transform bubbleRoot;

    [Header("Grid Settings")]
    [SerializeField] private int startRows = 5;
    [SerializeField] private int cols = 8;
    [SerializeField] private float cellSize = 0.72f;
    [SerializeField] private float rowHeight = 0.62f;
    [SerializeField] private float topY = 4.3f;
    [SerializeField] private float leftX = -3.15f;
    [SerializeField] private int maxRows = 20;

    [Header("Color Settings")]
    [SerializeField] private int randomColorCount = 4;

    private Dictionary<Vector2Int, Bubble> grid = new Dictionary<Vector2Int, Bubble>();

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateRandomGrid()
    {
        ClearBoard();

        if (bubblePrefab == null || bubbleRoot == null)
        {
            Debug.LogError("GridManager thiếu BubblePrefab hoặc BubbleRoot");
            return;
        }

        for (int row = 0; row < startRows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector2Int cell = new Vector2Int(row, col);
                Vector2 pos = CellToWorld(cell);

                GameObject obj = Instantiate(bubblePrefab, pos, Quaternion.identity, bubbleRoot);
                Bubble bubble = obj.GetComponent<Bubble>();
                BubbleProjectile projectile = obj.GetComponent<BubbleProjectile>();

                if (bubble == null || projectile == null)
                {
                    Debug.LogError("BubblePrefab thiếu Bubble hoặc BubbleProjectile");
                    continue;
                }

                BubbleColor color = (BubbleColor)Random.Range(0, randomColorCount);
                bubble.SetColor(color);
                bubble.isAttached = true;
                projectile.AttachToGrid();

                grid[cell] = bubble;
            }
        }
    }

    public void AttachProjectile(BubbleProjectile projectile)
    {
        if (projectile == null) return;

        Vector2 hitPos = projectile.transform.position;
        Vector2Int targetCell = FindBestAttachCell(hitPos);

        projectile.AttachToGrid();
        projectile.transform.SetParent(bubbleRoot);
        projectile.transform.position = CellToWorld(targetCell);
        projectile.Bubble.isAttached = true;

        grid[targetCell] = projectile.Bubble;

        int poppedCount;
        int droppedCount;
        bool exploded = TryPopCluster(targetCell, out poppedCount, out droppedCount);

        GameManager.Instance.NotifyShotResolved(exploded, poppedCount, droppedCount);
        GameManager.Instance.CheckLoseLine();
    }

    public bool HasBubbleBelow(float loseY)
    {
        foreach (var pair in grid)
        {
            if (pair.Value != null && pair.Value.transform.position.y <= loseY)
                return true;
        }
        return false;
    }

    private bool TryPopCluster(Vector2Int origin, out int poppedCount, out int droppedCount)
    {
        poppedCount = 0;
        droppedCount = 0;

        if (!grid.ContainsKey(origin))
            return false;

        List<Vector2Int> sameCluster = FindSameColorCluster(origin);

        if (sameCluster.Count < 3)
            return false;

        foreach (Vector2Int cell in sameCluster)
        {
            if (grid.ContainsKey(cell))
            {
                Bubble bubble = grid[cell];
                grid.Remove(cell);

                if (bubble != null)
                {
                    poppedCount++;
                    bubble.Pop();
                }
            }
        }

        droppedCount = DropFloatingBubbles();

        if (grid.Count == 0)
            GameManager.Instance.Win();

        return true;
    }

    private List<Vector2Int> FindSameColorCluster(Vector2Int start)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        if (!grid.ContainsKey(start))
            return result;

        BubbleColor targetColor = grid[start].colorId;
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            result.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!grid.ContainsKey(neighbor)) continue;
                if (visited.Contains(neighbor)) continue;
                if (grid[neighbor].colorId != targetColor) continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }

        return result;
    }

    private int DropFloatingBubbles()
    {
        HashSet<Vector2Int> connected = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        foreach (var pair in grid)
        {
            if (pair.Key.x == 0)
            {
                connected.Add(pair.Key);
                queue.Enqueue(pair.Key);
            }
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!grid.ContainsKey(neighbor)) continue;
                if (connected.Contains(neighbor)) continue;

                connected.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }

        List<Vector2Int> floating = new List<Vector2Int>();

        foreach (var pair in grid)
        {
            if (!connected.Contains(pair.Key))
                floating.Add(pair.Key);
        }

        int dropCount = 0;

        foreach (Vector2Int cell in floating)
        {
            Bubble bubble = grid[cell];
            grid.Remove(cell);

            if (bubble != null)
            {
                BubbleProjectile proj = bubble.GetComponent<BubbleProjectile>();
                if (proj != null)
                    proj.FallAway();
                else
                    Destroy(bubble.gameObject);

                dropCount++;
            }
        }

        return dropCount;
    }

    private Vector2Int FindBestAttachCell(Vector2 worldPos)
    {
        Vector2Int approx = WorldToCell(worldPos);

        float bestDistance = float.MaxValue;
        Vector2Int bestCell = new Vector2Int(0, 0);
        bool found = false;

        for (int row = approx.x - 2; row <= approx.x + 2; row++)
        {
            for (int col = approx.y - 2; col <= approx.y + 2; col++)
            {
                Vector2Int test = new Vector2Int(row, col);

                if (!IsInsideGrid(test)) continue;
                if (grid.ContainsKey(test)) continue;
                if (!IsValidAttachCell(test)) continue;

                Vector2 pos = CellToWorld(test);
                float dist = Vector2.Distance(worldPos, pos);

                // Không chọn ô nằm quá thấp so với điểm va chạm
                if (pos.y < worldPos.y - rowHeight * 0.75f)
                    continue;

                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestCell = test;
                    found = true;
                }
            }
        }

        // Fallback nếu không tìm thấy
        if (!found)
        {
            if (approx.x == 0)
                return approx;

            foreach (Vector2Int neighbor in GetNeighbors(approx))
            {
                if (IsInsideGrid(neighbor) && !grid.ContainsKey(neighbor))
                    return neighbor;
            }

            return approx;
        }

        return bestCell;
    }

    private bool IsValidAttachCell(Vector2Int cell)
    {
        if (cell.x == 0)
            return true;

        foreach (Vector2Int neighbor in GetNeighbors(cell))
        {
            if (grid.ContainsKey(neighbor))
                return true;
        }

        return false;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        bool oddRow = (cell.x % 2 == 1);

        Vector2Int[] evenOffsets =
        {
            new Vector2Int(-1, -1),
            new Vector2Int(-1,  0),
            new Vector2Int( 0, -1),
            new Vector2Int( 0,  1),
            new Vector2Int( 1, -1),
            new Vector2Int( 1,  0),
        };

        Vector2Int[] oddOffsets =
        {
            new Vector2Int(-1,  0),
            new Vector2Int(-1,  1),
            new Vector2Int( 0, -1),
            new Vector2Int( 0,  1),
            new Vector2Int( 1,  0),
            new Vector2Int( 1,  1),
        };

        Vector2Int[] offsets = oddRow ? oddOffsets : evenOffsets;

        foreach (Vector2Int offset in offsets)
        {
            Vector2Int neighbor = cell + offset;
            if (IsInsideGrid(neighbor))
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private Vector2 CellToWorld(Vector2Int cell)
    {
        float xOffset = (cell.x % 2 == 1) ? cellSize * 0.5f : 0f;
        float x = leftX + xOffset + cell.y * cellSize;
        float y = topY - cell.x * rowHeight;
        return new Vector2(x, y);
    }

    private Vector2Int WorldToCell(Vector2 worldPos)
    {
        int row = Mathf.RoundToInt((topY - worldPos.y) / rowHeight);
        row = Mathf.Clamp(row, 0, maxRows - 1);

        float xOffset = (row % 2 == 1) ? cellSize * 0.5f : 0f;
        int col = Mathf.RoundToInt((worldPos.x - leftX - xOffset) / cellSize);
        col = Mathf.Clamp(col, 0, cols - 1);

        return new Vector2Int(row, col);
    }

    private bool IsInsideGrid(Vector2Int cell)
    {
        return cell.x >= 0 && cell.y >= 0 && cell.y < cols && cell.x < maxRows;
    }

    private void ClearBoard()
    {
        foreach (var pair in grid)
        {
            if (pair.Value != null)
                Destroy(pair.Value.gameObject);
        }

        grid.Clear();
    }
}