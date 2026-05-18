using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [Header("Pipe Settings")]
    public GameObject pipePrefab;
    public float spawnRate = 1.6f;
    public float firstSpawnDelay = 0.3f;
    public float heightOffset = 1.5f;

    [Header("Spawn Position")]
    public float spawnOffsetX = 1.5f;

    private float timer;

    void Start()
    {
        timer = firstSpawnDelay;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.IsGameStarted()) return;
        if (GameManager.Instance.IsGameOver()) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnPipe();
            timer = spawnRate;
        }
    }

    void SpawnPipe()
    {
        if (pipePrefab == null)
        {
            Debug.LogError("Chưa gán Pipe Prefab vào PipeSpawner!");
            return;
        }

        float randomY = Random.Range(-heightOffset, heightOffset);

        float rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 0)).x;
        float spawnX = rightEdge + spawnOffsetX;

        Vector3 spawnPosition = new Vector3(spawnX, randomY, 0);

        Instantiate(pipePrefab, spawnPosition, Quaternion.identity);
    }
}