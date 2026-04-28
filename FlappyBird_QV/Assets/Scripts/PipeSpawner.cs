using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pipePairPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnX = 8f;
    [SerializeField] private float minY = -1.5f;
    [SerializeField] private float maxY = 1.5f;

    private float timer;

    private void Start()
    {
        timer = 0f;
        SpawnPipe();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        if (pipePairPrefab == null)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnPipe();
            timer = 0f;
        }
    }

    private void SpawnPipe()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(spawnX, randomY, 0f);
        Instantiate(pipePairPrefab, spawnPos, Quaternion.identity);
    }
}