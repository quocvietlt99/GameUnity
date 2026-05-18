using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public static Tetromino ActiveTetromino { get; set; }

    [Header("Piece")]
    [SerializeField] private TetrominoKind kind;
    [SerializeField] private bool canRotate = true;

    [Header("Timing")]
    [SerializeField] private float fallbackFallInterval = 0.8f;

    [Header("Visual Size")]
    [Tooltip("Độ đầy của block trong 1 ô lưới. 1 = vừa ô, 0.9 = nhỏ hơn ô một chút.")]
    [SerializeField] private float blockFill = 1.0f;

    [Tooltip("Tăng nếu sprite block vẫn bị nhỏ. Thử 1.2, 1.5, 2.0 nếu cần.")]
    [SerializeField] private float blockVisualMultiplier = 1.6f;

    public TetrominoKind Kind
    {
        get { return kind; }
        set { kind = value; }
    }

    private float fallTimer;
    private bool isLocked;
    private bool lastActionWasRotate;
    private GameObject ghostObject;

    private void Start()
    {
        ActiveTetromino = this;

        PreparePieceVisual(gameObject, kind, blockFill, blockVisualMultiplier);

        SnapRootToGrid();
        SnapBlocksToGrid();

        if (!IsValidPosition())
        {
            DebugInvalidSpawn();

            if (TetrisGameManager.Instance != null)
            {
                TetrisGameManager.Instance.GameOver();
            }

            isLocked = true;
            return;
        }

        RefreshGhost();
    }

    private void Update()
    {
        if (isLocked)
        {
            return;
        }

        if (TetrisGameManager.Instance != null && TetrisGameManager.Instance.IsGameOver)
        {
            return;
        }

        HandleKeyboardInput();
        HandleAutoFall();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }

        if (
            Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.X)
        )
        {
            RotateClockwise();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            RotateCounterClockwise();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            SoftDrop();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Spawner.Instance != null)
            {
                Spawner.Instance.HoldCurrentPiece();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (TetrisGameManager.Instance != null)
            {
                TetrisGameManager.Instance.Restart();
            }
        }
    }

    private void HandleAutoFall()
    {
        fallTimer += Time.deltaTime;

        float interval = fallbackFallInterval;

        if (TetrisGameManager.Instance != null)
        {
            interval = TetrisGameManager.Instance.GetFallInterval();
        }

        if (fallTimer >= interval)
        {
            fallTimer = 0f;
            MoveDown();
        }
    }

    public void MoveLeft()
    {
        TryMove(Vector3.left * GameGrid.Step, true);
    }

    public void MoveRight()
    {
        TryMove(Vector3.right * GameGrid.Step, true);
    }

    public void MoveDown()
    {
        if (!TryMove(Vector3.down * GameGrid.Step, false))
        {
            LockPiece();
        }
    }

    public void SoftDrop()
    {
        if (TryMove(Vector3.down * GameGrid.Step, false))
        {
            if (TetrisGameManager.Instance != null)
            {
                TetrisGameManager.Instance.AddSoftDropScore();
            }
        }
        else
        {
            LockPiece();
        }
    }

    public void HardDrop()
    {
        if (isLocked)
        {
            return;
        }

        int distance = 0;

        while (TryMove(Vector3.down * GameGrid.Step, false, false))
        {
            distance++;
        }

        if (TetrisGameManager.Instance != null)
        {
            TetrisGameManager.Instance.AddHardDropScore(distance);
        }

        if (TetrisAudioManager.Instance != null)
        {
            TetrisAudioManager.Instance.PlayHardDrop();
        }

        LockPiece();
    }

    public void RotateClockwise()
    {
        TryRotate(-90f);
    }

    public void RotateCounterClockwise()
    {
        TryRotate(90f);
    }

    private bool TryMove(Vector3 offset, bool playAudio, bool refreshGhost = true)
    {
        if (isLocked)
        {
            return false;
        }

        Vector3 oldPosition = transform.position;

        transform.position += offset;
        SnapRootToGrid();

        if (!IsValidPosition())
        {
            transform.position = oldPosition;
            return false;
        }

        lastActionWasRotate = false;

        if (refreshGhost)
        {
            RefreshGhost();
        }

        if (playAudio && TetrisAudioManager.Instance != null)
        {
            TetrisAudioManager.Instance.PlayMove();
        }

        return true;
    }

    private void TryRotate(float angle)
    {
        if (!canRotate || isLocked)
        {
            return;
        }

        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        transform.Rotate(0f, 0f, angle);

        Vector3[] kicks = GetWallKickOffsets();

        for (int i = 0; i < kicks.Length; i++)
        {
            transform.position = oldPosition + kicks[i];
            SnapRootToGrid();

            if (IsValidPosition())
            {
                SnapBlocksToGrid();

                lastActionWasRotate = true;
                RefreshGhost();

                if (TetrisAudioManager.Instance != null)
                {
                    TetrisAudioManager.Instance.PlayRotate();
                }

                return;
            }
        }

        transform.position = oldPosition;
        transform.rotation = oldRotation;
    }

    private Vector3[] GetWallKickOffsets()
    {
        float s = GameGrid.Step;

        if (kind == TetrominoKind.I)
        {
            return new[]
            {
                Vector3.zero,
                Vector3.right * s,
                Vector3.left * s,
                Vector3.right * s * 2f,
                Vector3.left * s * 2f,
                Vector3.up * s,
                Vector3.down * s
            };
        }

        return new[]
        {
            Vector3.zero,
            Vector3.right * s,
            Vector3.left * s,
            Vector3.up * s,
            new Vector3(s, s, 0f),
            new Vector3(-s, s, 0f)
        };
    }

    private bool IsValidPosition()
    {
        Transform[] blocks = GetBlocks();

        if (blocks.Length != 4)
        {
            Debug.LogWarning(name + " phải có đúng 4 SpriteRenderer block. Hiện có: " + blocks.Length);
            return false;
        }

        for (int i = 0; i < blocks.Length; i++)
        {
            Vector2Int gridPosition = GameGrid.WorldToGrid(blocks[i].position);

            int x = gridPosition.x;
            int y = gridPosition.y;

            if (!GameGrid.IsInsideHorizontal(x))
            {
                return false;
            }

            if (y < 0)
            {
                return false;
            }

            if (y >= GameGrid.Height)
            {
                continue;
            }

            if (GameGrid.Grid[x, y] != null)
            {
                return false;
            }
        }

        if (HasDuplicateGridCells())
        {
            return false;
        }

        return true;
    }

    private void LockPiece()
    {
        if (isLocked)
        {
            return;
        }

        isLocked = true;

        SnapBlocksToGrid();
        DestroyGhost();

        bool tSpin = DetectTSpin();

        Transform[] blocks = GetBlocks();

        for (int i = 0; i < blocks.Length; i++)
        {
            Transform block = blocks[i];

            Vector2Int gridPosition = GameGrid.WorldToGrid(block.position);

            int x = gridPosition.x;
            int y = gridPosition.y;

            if (y >= GameGrid.Height)
            {
                if (TetrisGameManager.Instance != null)
                {
                    TetrisGameManager.Instance.GameOver();
                }

                return;
            }

            if (GameGrid.IsInsideGrid(x, y))
            {
                block.SetParent(null);
                block.position = GameGrid.GridToWorld(x, y);
                GameGrid.Grid[x, y] = block;
            }
        }

        int clearedLines = GameGrid.ClearFullLines();

        if (TetrisGameManager.Instance != null)
        {
            TetrisGameManager.Instance.OnPieceLocked(clearedLines, tSpin);
        }

        if (TetrisAudioManager.Instance != null)
        {
            if (clearedLines > 0)
            {
                TetrisAudioManager.Instance.PlayLineClear();
            }
            else
            {
                TetrisAudioManager.Instance.PlayLock();
            }
        }

        if (ActiveTetromino == this)
        {
            ActiveTetromino = null;
        }

        Destroy(gameObject);

        if (Spawner.Instance != null)
        {
            Spawner.Instance.SpawnNext();
        }
    }

    private bool DetectTSpin()
    {
        if (kind != TetrominoKind.T || !lastActionWasRotate)
        {
            return false;
        }

        Vector2Int pivot = GameGrid.WorldToGrid(transform.position);

        Vector2Int[] corners =
        {
            pivot + new Vector2Int(-1, -1),
            pivot + new Vector2Int(1, -1),
            pivot + new Vector2Int(-1, 1),
            pivot + new Vector2Int(1, 1)
        };

        int blockedCorners = 0;

        for (int i = 0; i < corners.Length; i++)
        {
            if (IsCornerBlocked(corners[i]))
            {
                blockedCorners++;
            }
        }

        return blockedCorners >= 3;
    }

    private bool IsCornerBlocked(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= GameGrid.Width || cell.y < 0)
        {
            return true;
        }

        if (cell.y >= GameGrid.Height)
        {
            return false;
        }

        return GameGrid.Grid[cell.x, cell.y] != null;
    }

    private void RefreshGhost()
    {
        DestroyGhost();

        if (isLocked)
        {
            return;
        }

        int distance = 0;

        while (CanExistAt(transform.position + Vector3.down * GameGrid.Step * (distance + 1), transform.rotation))
        {
            distance++;
        }

        ghostObject = Instantiate(gameObject, transform.position + Vector3.down * GameGrid.Step * distance, transform.rotation);
        ghostObject.name = "Ghost_" + name;

        Tetromino ghostTetromino = ghostObject.GetComponent<Tetromino>();
        if (ghostTetromino != null)
        {
            ghostTetromino.enabled = false;
        }

        SetGhostVisual(ghostObject);
    }

    private bool CanExistAt(Vector3 position, Quaternion rotation)
    {
        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        transform.position = position;
        transform.rotation = rotation;

        bool valid = IsValidPosition();

        transform.position = oldPosition;
        transform.rotation = oldRotation;

        return valid;
    }

    private void DestroyGhost()
    {
        if (ghostObject != null)
        {
            Destroy(ghostObject);
            ghostObject = null;
        }
    }

    private void SetGhostVisual(GameObject target)
    {
        SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Color color = renderers[i].color;
            color.a = 0.25f;
            renderers[i].color = color;
            renderers[i].sortingOrder = -10;
        }
    }

    private Transform[] GetBlocks()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        Transform[] blocks = new Transform[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            blocks[i] = renderers[i].transform;
        }

        return blocks;
    }

    private void SnapRootToGrid()
    {
        Vector2Int gridPosition = GameGrid.WorldToGrid(transform.position);
        transform.position = GameGrid.GridToWorld(gridPosition.x, gridPosition.y);
    }

    private void SnapBlocksToGrid()
    {
        Transform[] blocks = GetBlocks();

        for (int i = 0; i < blocks.Length; i++)
        {
            Vector2Int gridPosition = GameGrid.WorldToGrid(blocks[i].position);

            int x = gridPosition.x;
            int y = gridPosition.y;

            blocks[i].position = GameGrid.GridToWorld(x, y);
        }
    }

    private bool HasDuplicateGridCells()
    {
        Transform[] blocks = GetBlocks();

        for (int i = 0; i < blocks.Length; i++)
        {
            Vector2Int a = GameGrid.WorldToGrid(blocks[i].position);

            for (int j = i + 1; j < blocks.Length; j++)
            {
                Vector2Int b = GameGrid.WorldToGrid(blocks[j].position);

                if (a == b)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DebugInvalidSpawn()
    {
        Debug.LogWarning("---- Invalid Tetromino Spawn Debug ----");
        Debug.LogWarning("Tetromino: " + name + " | Kind: " + kind + " | Root: " + transform.position);

        Transform[] blocks = GetBlocks();

        for (int i = 0; i < blocks.Length; i++)
        {
            Vector2Int gridPosition = GameGrid.WorldToGrid(blocks[i].position);

            Debug.LogWarning(
                blocks[i].name +
                " | local=" + blocks[i].localPosition +
                " | world=" + blocks[i].position +
                " | grid=" + gridPosition
            );
        }

        Debug.LogWarning("Block count = " + blocks.Length);
        Debug.LogWarning("---------------------------------------");
    }

    public static void ApplyCanonicalShapeTo(GameObject target, TetrominoKind shapeKind)
    {
        PreparePieceVisual(target, shapeKind, 1.0f, 1.6f);
    }

    private static void PreparePieceVisual(GameObject target, TetrominoKind shapeKind, float fill, float visualMultiplier)
    {
        if (target == null)
        {
            return;
        }

        target.transform.localScale = Vector3.one;

        ResetNonBlockParentsScale(target);

        SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length != 4)
        {
            Debug.LogWarning(target.name + " phải có đúng 4 SpriteRenderer. Hiện có: " + renderers.Length);
            return;
        }

        Vector3[] positions = GetCanonicalLocalPositions(shapeKind);

        for (int i = 0; i < renderers.Length; i++)
        {
            Transform block = renderers[i].transform;

            block.localPosition = positions[i];
            block.localRotation = Quaternion.identity;

            FitBlockVisualToCell(renderers[i], fill, visualMultiplier);
        }
    }

    private static void ResetNonBlockParentsScale(GameObject target)
    {
        Transform[] allTransforms = target.GetComponentsInChildren<Transform>();

        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform current = allTransforms[i];

            if (current == target.transform)
            {
                current.localScale = Vector3.one;
                continue;
            }

            SpriteRenderer renderer = current.GetComponent<SpriteRenderer>();

            if (renderer == null)
            {
                current.localScale = Vector3.one;
            }
        }
    }

    private static void FitBlockVisualToCell(SpriteRenderer renderer, float fill, float visualMultiplier)
    {
        if (renderer == null || renderer.sprite == null)
        {
            return;
        }

        Vector2 spriteSize = renderer.sprite.bounds.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
        {
            renderer.transform.localScale = Vector3.one;
            return;
        }

        float targetWidth = GameGrid.Step * fill * visualMultiplier;
        float targetHeight = GameGrid.Step * fill * visualMultiplier;

        float scaleX = targetWidth / spriteSize.x;
        float scaleY = targetHeight / spriteSize.y;

        renderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    private static Vector3[] GetCanonicalLocalPositions(TetrominoKind shapeKind)
    {
        float s = GameGrid.Step;

        switch (shapeKind)
        {
            case TetrominoKind.I:
                return new[]
                {
                    new Vector3(-2f * s, 0f, 0f),
                    new Vector3(-1f * s, 0f, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(1f * s, 0f, 0f)
                };

            case TetrominoKind.O:
                return new[]
                {
                    new Vector3(0f, 0f, 0f),
                    new Vector3(1f * s, 0f, 0f),
                    new Vector3(0f, 1f * s, 0f),
                    new Vector3(1f * s, 1f * s, 0f)
                };

            case TetrominoKind.T:
                return new[]
                {
                    new Vector3(-1f * s, 0f, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(1f * s, 0f, 0f),
                    new Vector3(0f, 1f * s, 0f)
                };

            case TetrominoKind.S:
                return new[]
                {
                    new Vector3(-1f * s, 0f, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(0f, 1f * s, 0f),
                    new Vector3(1f * s, 1f * s, 0f)
                };

            case TetrominoKind.Z:
                return new[]
                {
                    new Vector3(-1f * s, 1f * s, 0f),
                    new Vector3(0f, 1f * s, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(1f * s, 0f, 0f)
                };

            case TetrominoKind.J:
                return new[]
                {
                    new Vector3(-1f * s, 1f * s, 0f),
                    new Vector3(-1f * s, 0f, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(1f * s, 0f, 0f)
                };

            case TetrominoKind.L:
                return new[]
                {
                    new Vector3(-1f * s, 0f, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(1f * s, 0f, 0f),
                    new Vector3(1f * s, 1f * s, 0f)
                };

            default:
                return new[]
                {
                    Vector3.zero,
                    Vector3.right * s,
                    Vector3.up * s,
                    new Vector3(s, s, 0f)
                };
        }
    }

    private void OnDestroy()
    {
        DestroyGhost();
    }
}