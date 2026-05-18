using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }

    [Header("Spawn Cell")]
    [SerializeField] private int spawnX = 5;
    [SerializeField] private int spawnY = 18;

    [Header("Tetromino Prefabs - đúng thứ tự I, O, T, S, Z, J, L")]
    [SerializeField] private GameObject[] tetrominoPrefabs = new GameObject[7];

    [Header("Next Queue Preview")]
    [SerializeField] private TetrisPreviewPanel[] nextPreviews = new TetrisPreviewPanel[3];

    [Header("Hold Preview")]
    [SerializeField] private TetrisPreviewPanel holdPreview;

    private readonly Queue<TetrominoKind> nextQueue = new Queue<TetrominoKind>();
    private readonly List<TetrominoKind> bag = new List<TetrominoKind>();

    private bool hasHoldPiece;
    private TetrominoKind holdKind;
    private bool canHoldThisTurn = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SnapToSpawnCell();
        FillQueueIfNeeded();
        RefreshAllPreviews();
        SpawnNext();
    }

    private void SnapToSpawnCell()
    {
        transform.position = GameGrid.GridToWorld(spawnX, spawnY);
    }

    public void SpawnNext()
    {
        if (TetrisGameManager.Instance != null && TetrisGameManager.Instance.IsGameOver)
        {
            return;
        }

        SnapToSpawnCell();
        FillQueueIfNeeded();

        TetrominoKind nextKind = nextQueue.Dequeue();

        FillQueueIfNeeded();
        RefreshAllPreviews();

        SpawnSpecific(nextKind, true);
    }

    private void SpawnSpecific(TetrominoKind kind, bool allowHold)
    {
        SnapToSpawnCell();

        GameObject prefab = GetPrefab(kind);

        if (prefab == null)
        {
            Debug.LogError("Thiếu Prefab cho khối: " + kind);
            return;
        }

        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);

        Tetromino tetromino = obj.GetComponent<Tetromino>();

        if (tetromino != null)
        {
            tetromino.Kind = kind;
        }
        else
        {
            Debug.LogError("Prefab " + prefab.name + " chưa có script Tetromino.");
        }

        canHoldThisTurn = allowHold;
    }

    public void HoldCurrentPiece()
    {
        Tetromino active = Tetromino.ActiveTetromino;

        if (active == null || !canHoldThisTurn)
        {
            return;
        }

        TetrominoKind currentKind = active.Kind;

        Destroy(active.gameObject);
        Tetromino.ActiveTetromino = null;

        if (!hasHoldPiece)
        {
            holdKind = currentKind;
            hasHoldPiece = true;
            SpawnNext();
        }
        else
        {
            TetrominoKind pieceToSpawn = holdKind;
            holdKind = currentKind;

            SpawnSpecific(pieceToSpawn, false);
        }

        canHoldThisTurn = false;

        if (TetrisAudioManager.Instance != null)
        {
            TetrisAudioManager.Instance.PlayHold();
        }

        RefreshAllPreviews();
    }

    public GameObject GetPrefab(TetrominoKind kind)
    {
        int index = (int)kind;

        if (tetrominoPrefabs == null)
        {
            return null;
        }

        if (index < 0 || index >= tetrominoPrefabs.Length)
        {
            return null;
        }

        return tetrominoPrefabs[index];
    }

    private void FillQueueIfNeeded()
    {
        while (nextQueue.Count < 7)
        {
            AddSevenBag();
        }
    }

    private void AddSevenBag()
    {
        bag.Clear();

        bag.Add(TetrominoKind.I);
        bag.Add(TetrominoKind.O);
        bag.Add(TetrominoKind.T);
        bag.Add(TetrominoKind.S);
        bag.Add(TetrominoKind.Z);
        bag.Add(TetrominoKind.J);
        bag.Add(TetrominoKind.L);

        for (int i = 0; i < bag.Count; i++)
        {
            int randomIndex = Random.Range(i, bag.Count);

            TetrominoKind temp = bag[i];
            bag[i] = bag[randomIndex];
            bag[randomIndex] = temp;
        }

        for (int i = 0; i < bag.Count; i++)
        {
            nextQueue.Enqueue(bag[i]);
        }
    }

    private TetrominoKind[] GetNextKinds(int count)
    {
        TetrominoKind[] allPieces = nextQueue.ToArray();

        int resultLength = Mathf.Min(count, allPieces.Length);
        TetrominoKind[] result = new TetrominoKind[resultLength];

        for (int i = 0; i < resultLength; i++)
        {
            result[i] = allPieces[i];
        }

        return result;
    }

    private void RefreshAllPreviews()
    {
        RefreshNextPreviews();
        RefreshHoldPreview();
    }

    private void RefreshNextPreviews()
    {
        if (nextPreviews == null || nextPreviews.Length == 0)
        {
            Debug.LogWarning("Spawner chưa gán Next Previews.");
            return;
        }

        TetrominoKind[] nextKinds = GetNextKinds(nextPreviews.Length);

        for (int i = 0; i < nextPreviews.Length; i++)
        {
            if (nextPreviews[i] == null)
            {
                Debug.LogWarning("Next Preview Element " + i + " chưa được gán.");
                continue;
            }

            if (i < nextKinds.Length)
            {
                GameObject prefab = GetPrefab(nextKinds[i]);

                if (prefab == null)
                {
                    Debug.LogWarning("Không có prefab cho preview: " + nextKinds[i]);
                    nextPreviews[i].Clear();
                    continue;
                }

                nextPreviews[i].Render(prefab, nextKinds[i]);
            }
            else
            {
                nextPreviews[i].Clear();
            }
        }
    }

    private void RefreshHoldPreview()
    {
        if (holdPreview == null)
        {
            return;
        }

        if (!hasHoldPiece)
        {
            holdPreview.Clear();
            return;
        }

        GameObject prefab = GetPrefab(holdKind);
        holdPreview.Render(prefab, holdKind);
    }
}