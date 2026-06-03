using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAndObstaclesManager_Finished : MonoBehaviour
{
    public static PlatformAndObstaclesManager_Finished instance;

    [Header("Ground - Single Ground Old Support")]
    public GameObject ground;

    [Header("Ground Blocks - For 2 Ground Blocks")]
    public GameObject[] groundBlocks;

    [Tooltip("Chiều dài 1 khối mặt đất. Nếu mặt đất bị hở/chồng, chỉnh số này.")]
    public float groundSingleBlockLength = 1.7912f;

    [Header("Movement")]
    public float speed = 5f;

    [Header("Obstacle Prefabs")]
    [Tooltip("Kéo Obstacle1, Obstacle2 và BirdObstacle prefab vào đây.")]
    public GameObject[] obstacles;

    [Header("Obstacle Spawn")]
    public bool useCameraRightEdge = true;
    public float spawnOffsetFromCameraRight = 1.2f;
    public float spawnXPosition = 6.5f;

    [Tooltip("Độ cao mặc định cho xương rồng.")]
    public float cactusYPosition = -0.147f;

    public float obstacleZPosition = -1f;

    [Header("Distance Between Obstacles")]
    [Tooltip("Khoảng cách tối thiểu giữa 2 chướng ngại vật.")]
    public float minDistanceBetweenObstacles = 3.5f;

    [Tooltip("Khoảng cách tối đa giữa 2 chướng ngại vật.")]
    public float maxDistanceBetweenObstacles = 6f;

    [Header("Obstacle Render")]
    public string obstacleSortingLayerName = "Default";
    public int obstacleSortingOrder = 50;

    [Header("Auto Spawn")]
    public float firstSpawnDelay = 0.5f;

    private float spawnTimer;

    void Start()
    {
        if (PlatformAndObstaclesManager_Finished.instance == null)
        {
            PlatformAndObstaclesManager_Finished.instance = this;
        }

        SetupGroundBlocksFallback();
        SetNextSpawnDelay(firstSpawnDelay);
    }

    void Update()
    {
        if (GameManagerScript_Finished.instance == null)
        {
            return;
        }

        if (!GameManagerScript_Finished.instance.gameRunning)
        {
            return;
        }

        MoveGround();
        HandleAutoSpawn();
    }

    private void SetupGroundBlocksFallback()
    {
        if ((groundBlocks == null || groundBlocks.Length == 0) && ground != null)
        {
            groundBlocks = new GameObject[] { ground };
        }
    }

    private float GetFinalSpeed()
    {
        float multiplier = 1f;

        if (GameManagerScript_Finished.instance != null)
        {
            multiplier = GameManagerScript_Finished.instance.GetSpeedMultiplier();
        }

        return speed * multiplier;
    }

    private void MoveGround()
    {
        if (groundBlocks != null && groundBlocks.Length > 0)
        {
            MoveGroundBlocks();
            return;
        }

        MoveSingleGround();
    }

    private void MoveSingleGround()
    {
        if (ground == null)
        {
            return;
        }

        float finalSpeed = GetFinalSpeed();

        if (ground.transform.position.x > -groundSingleBlockLength)
        {
            ground.transform.Translate(Vector3.left * Time.deltaTime * finalSpeed);
        }
        else
        {
            ground.transform.position = new Vector3(0f, ground.transform.position.y, ground.transform.position.z);
        }
    }

    private void MoveGroundBlocks()
    {
        float finalSpeed = GetFinalSpeed();

        for (int i = 0; i < groundBlocks.Length; i++)
        {
            if (groundBlocks[i] == null)
            {
                continue;
            }

            groundBlocks[i].transform.Translate(Vector3.left * Time.deltaTime * finalSpeed);
        }

        for (int i = 0; i < groundBlocks.Length; i++)
        {
            if (groundBlocks[i] == null)
            {
                continue;
            }

            if (groundBlocks[i].transform.position.x <= -groundSingleBlockLength)
            {
                MoveGroundBlockToRightSide(groundBlocks[i]);
            }
        }
    }

    private void MoveGroundBlockToRightSide(GameObject blockToMove)
    {
        float rightMostX = GetRightMostGroundX();

        Vector3 newPosition = blockToMove.transform.position;
        newPosition.x = rightMostX + groundSingleBlockLength;

        blockToMove.transform.position = newPosition;
    }

    private float GetRightMostGroundX()
    {
        float rightMostX = float.MinValue;

        if (groundBlocks == null)
        {
            return 0f;
        }

        for (int i = 0; i < groundBlocks.Length; i++)
        {
            if (groundBlocks[i] == null)
            {
                continue;
            }

            if (groundBlocks[i].transform.position.x > rightMostX)
            {
                rightMostX = groundBlocks[i].transform.position.x;
            }
        }

        if (rightMostX == float.MinValue)
        {
            rightMostX = 0f;
        }

        return rightMostX;
    }

    private void HandleAutoSpawn()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnNew();

            float finalSpeed = GetFinalSpeed();

            if (finalSpeed <= 0f)
            {
                finalSpeed = speed;
            }

            float randomDistance = Random.Range(minDistanceBetweenObstacles, maxDistanceBetweenObstacles);
            float nextSpawnDelay = randomDistance / finalSpeed;

            SetNextSpawnDelay(nextSpawnDelay);
        }
    }

    private void SetNextSpawnDelay(float delay)
    {
        spawnTimer = delay;
    }

    public void SpawnNew()
    {
        if (GameManagerScript_Finished.instance == null ||
            !GameManagerScript_Finished.instance.gameRunning)
        {
            return;
        }

        if (obstacles == null || obstacles.Length == 0)
        {
            Debug.LogWarning("PlatformAndObstaclesManager_Finished: Chưa kéo prefab obstacle vào mảng Obstacles.");
            return;
        }

        int randomIndex = Random.Range(0, obstacles.Length);

        if (obstacles[randomIndex] == null)
        {
            Debug.LogWarning("PlatformAndObstaclesManager_Finished: Có phần tử trong Obstacles đang bị None.");
            return;
        }

        GameObject selectedPrefab = obstacles[randomIndex];

        float spawnX = GetSpawnXPosition();
        float spawnY = GetSpawnYPosition(selectedPrefab);

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, obstacleZPosition);

        GameObject newObstacle = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        PrepareObstacle(newObstacle);

        Debug.Log("SPAWN OBSTACLE: " + newObstacle.name + " at " + spawnPosition);
    }

    private float GetSpawnXPosition()
    {
        if (!useCameraRightEdge || Camera.main == null)
        {
            return spawnXPosition;
        }

        Vector3 rightEdgeWorldPosition = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        return rightEdgeWorldPosition.x + spawnOffsetFromCameraRight;
    }

    private float GetSpawnYPosition(GameObject prefab)
    {
        if (prefab == null)
        {
            return cactusYPosition;
        }

        BirdObstacle_Finished birdObstacle = prefab.GetComponent<BirdObstacle_Finished>();

        if (birdObstacle != null)
        {
            return birdObstacle.GetRandomYPosition();
        }

        return cactusYPosition;
    }

    private void PrepareObstacle(GameObject obstacle)
    {
        if (obstacle == null)
        {
            return;
        }

        obstacle.tag = "Obstacle";

        MoveObject_Finished moveObject = obstacle.GetComponent<MoveObject_Finished>();

        if (moveObject == null)
        {
            moveObject = obstacle.AddComponent<MoveObject_Finished>();
        }

        moveObject.speed = speed;

        ObstacleBlock_Finished obstacleBlock = obstacle.GetComponent<ObstacleBlock_Finished>();

        if (obstacleBlock == null)
        {
            obstacle.AddComponent<ObstacleBlock_Finished>();
        }

        ForceRenderInFront(obstacle);
        ForceColliderTriggerIfBird(obstacle);
    }

    private void ForceRenderInFront(GameObject obstacle)
    {
        SpriteRenderer[] renderers = obstacle.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sortingLayerName = obstacleSortingLayerName;
            renderers[i].sortingOrder = obstacleSortingOrder;
        }
    }

    private void ForceColliderTriggerIfBird(GameObject obstacle)
    {
        BirdObstacle_Finished birdObstacle = obstacle.GetComponent<BirdObstacle_Finished>();

        if (birdObstacle == null)
        {
            return;
        }

        BoxCollider2D boxCollider = obstacle.GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            boxCollider = obstacle.AddComponent<BoxCollider2D>();
        }

        boxCollider.isTrigger = true;
    }
}