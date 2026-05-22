using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float resetX = 10f;
    public float leftLimitX = -10f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.IsPlaying) return;

        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        if (transform.position.x <= leftLimitX)
        {
            ResetObstacle();
        }
    }

    private void ResetObstacle()
    {
        float randomDistance = Random.Range(0f, 4f);
        float randomY = transform.position.y;

        transform.position = new Vector3(
            resetX + randomDistance,
            randomY,
            transform.position.z
        );
    }
}