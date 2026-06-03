using UnityEngine;

public class EndlessGroundController : MonoBehaviour
{
    [Header("Ground Tiles")]
    public Transform[] groundTiles;

    [Header("Move Settings")]
    public float moveSpeed = 2.8f;
    public float tileWidth = 3f;
    public float leftBoundary = -6f;

    private void Start()
    {
        if (groundTiles == null || groundTiles.Length == 0)
        {
            Debug.LogWarning("Chưa gắn groundTiles cho EndlessGroundController.");
        }
    }

    private void Update()
    {
        if (AutoGameManager.Instance != null && !AutoGameManager.Instance.isPlaying)
            return;

        MoveGround();
        LoopGround();
    }

    private void MoveGround()
    {
        float speed = moveSpeed;

        if (AutoGameManager.Instance != null)
        {
            speed = AutoGameManager.Instance.worldSpeed;
        }

        foreach (Transform tile in groundTiles)
        {
            if (tile == null) continue;

            tile.position += Vector3.left * speed * Time.deltaTime;
        }
    }

    private void LoopGround()
    {
        foreach (Transform tile in groundTiles)
        {
            if (tile == null) continue;

            if (tile.position.x <= leftBoundary)
            {
                Transform rightMostTile = GetRightMostTile();

                Vector3 newPosition = tile.position;
                newPosition.x = rightMostTile.position.x + tileWidth;
                tile.position = newPosition;
            }
        }
    }

    private Transform GetRightMostTile()
    {
        Transform rightMost = groundTiles[0];

        foreach (Transform tile in groundTiles)
        {
            if (tile == null) continue;

            if (tile.position.x > rightMost.position.x)
            {
                rightMost = tile;
            }
        }

        return rightMost;
    }
}