using UnityEngine;

public class PipeMove : MonoBehaviour
{
    public float speed = 2.5f;
    public float destroyOffsetX = 1.5f;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.IsGameStarted()) return;
        if (GameManager.Instance.IsGameOver()) return;

        transform.Translate(Vector2.left * speed * Time.deltaTime);

        float leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).x;

        if (transform.position.x < leftEdge - destroyOffsetX)
        {
            Destroy(gameObject);
        }
    }
}