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
    public GameObject[] obstacles;

    [Header("Obstacle Spawn")]
    public bool useCameraRightEdge = true;
    public float spawnOffsetFromCameraRight = 1.2f;
    public float spawnXPosition = 6.5f;
    public float cactusYPosition = -0.147f;
    public float obstacleZPosition = -1f;

    [Header("Distance Between Obstacles")]
    [Tooltip("Khoảng cách tối thiểu giữa 2 cây xương rồng. Số càng lớn thì cây càng cách xa nhau.")]
    public float minDistanceBetweenObstacles = 3.0f;

    [Tooltip("Khoảng cách tối đa giữa 2 cây xương rồng. Số càng lớn thì cây sinh thưa hơn.")]
    public float maxDistanceBetweenObstacles = 5.5f;

    [Header("Obstacle Render")]
    public string obstacleSortingLayerName = "Default";
    public int obstacleSortingOrder = 50;

    [Header("Auto Spawn")]
    public float firstSpawnDelay = 0.5f;

    private float spawnTimer;
    private float nextSpawnDelay;

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

            nextSpawnDelay = randomDistance / finalSpeed;
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
            Debug.LogWarning("PlatformAndObstaclesManager_Finished: Chua keo prefab xuong rong vao Obstacles.");
            return;
        }

        int randomIndex = Random.Range(0, obstacles.Length);

        if (obstacles[randomIndex] == null)
        {
            Debug.LogWarning("PlatformAndObstaclesManager_Finished: Mot phan tu trong Obstacles dang bi None.");
            return;
        }

        float spawnX = GetSpawnXPosition();
        Vector3 spawnPosition = new Vector3(spawnX, cactusYPosition, obstacleZPosition);

        GameObject newObstacle = Instantiate(obstacles[randomIndex], spawnPosition, Quaternion.identity);

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

        SpriteRenderer[] renderers = obstacle.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sortingLayerName = obstacleSortingLayerName;
            renderers[i].sortingOrder = obstacleSortingOrder;
        }
    }
}