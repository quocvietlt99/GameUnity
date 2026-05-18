using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject pipePrefab;

    void Start()
    {
        InvokeRepeating("SpawnPipe", 1f, 2f);
    }

    void SpawnPipe()
    {
        float y = Random.Range(-1.5f, 1.5f);
        Instantiate(pipePrefab, new Vector3(4, y, 0), Quaternion.identity);
    }
}