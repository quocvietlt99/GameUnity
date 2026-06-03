using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBirdSpawner_Finished : MonoBehaviour
{
    [Header("Bird Prefabs")]
    [Tooltip("Kéo 3 prefab chim vào đây.")]
    public GameObject[] birdPrefabs;

    [Header("Spawn Position")]
    public bool useCameraRightEdge = true;
    public float spawnOffsetFromCameraRight = 1.2f;
    public float spawnXPosition = 6.5f;
    public float spawnZPosition = -1f;

    [Tooltip("Các độ cao chim bay. Có thể chỉnh trong Inspector.")]
    public float[] birdYPositions = new float[]
    {
        0.2f,
        0.8f,
        1.3f
    };

    [Header("Movement")]
    public float birdSpeed = 5f;

    [Header("Spawn Time")]
    public float firstSpawnDelay = 2f;
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 6f;

    [Header("Random Chance")]
    [Range(0f, 1f)]
    public float spawnChance = 0.75f;

    private float spawnTimer;

    private void Start()
    {
        ResetSpawnTimer(firstSpawnDelay);
    }

    private void Update()
    {
        if (!IsGameRunning())
        {
            return;
        }

        HandleSpawn();
    }

    private bool IsGameRunning()
    {
        return GameManagerScript_Finished.instance != null &&
               GameManagerScript_Finished.instance.gameRunning;
    }

    private void HandleSpawn()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer > 0f)
        {
            return;
        }

        if (Random.value <= spawnChance)
        {
            SpawnBird();
        }

        ResetSpawnTimer(Random.Range(minSpawnInterval, maxSpawnInterval));
    }

    private void ResetSpawnTimer(float delay)
    {
        spawnTimer = delay;
    }

    private void SpawnBird()
    {
        if (birdPrefabs == null || birdPrefabs.Length == 0)
        {
            Debug.LogWarning("RandomBirdSpawner_Finished: Chưa kéo 3 prefab chim vào Bird Prefabs.");
            return;
        }

        int randomIndex = Random.Range(0, birdPrefabs.Length);

        if (birdPrefabs[randomIndex] == null)
        {
            Debug.LogWarning("RandomBirdSpawner_Finished: Có prefab chim đang bị None.");
            return;
        }

        float spawnX = GetSpawnXPosition();
        float spawnY = GetRandomBirdY();

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZPosition);

        GameObject bird = Instantiate(birdPrefabs[randomIndex], spawnPosition, Quaternion.identity);

        PrepareBird(bird);

        Debug.Log("SPAWN BIRD: " + bird.name + " at " + spawnPosition);
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

    private float GetRandomBirdY()
    {
        if (birdYPositions == null || birdYPositions.Length == 0)
        {
            return 0.8f;
        }

        int randomIndex = Random.Range(0, birdYPositions.Length);
        return birdYPositions[randomIndex];
    }

    private void PrepareBird(GameObject bird)
    {
        if (bird == null)
        {
            return;
        }

        bird.tag = "Obstacle";

        BirdObstacle_Finished birdObstacle = bird.GetComponent<BirdObstacle_Finished>();

        if (birdObstacle == null)
        {
            birdObstacle = bird.AddComponent<BirdObstacle_Finished>();
        }

        birdObstacle.SetSpeed(birdSpeed);

        BoxCollider2D boxCollider = bird.GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            boxCollider = bird.AddComponent<BoxCollider2D>();
        }

        boxCollider.isTrigger = true;

        ObstacleBlock_Finished obstacleBlock = bird.GetComponent<ObstacleBlock_Finished>();

        if (obstacleBlock == null)
        {
            bird.AddComponent<ObstacleBlock_Finished>();
        }

        SpriteRenderer[] renderers = bird.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sortingLayerName = "Default";
            renderers[i].sortingOrder = 50;
        }
    }
}