using UnityEngine;

public class PipePairMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float destroyX = -10f;

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (transform.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }
}