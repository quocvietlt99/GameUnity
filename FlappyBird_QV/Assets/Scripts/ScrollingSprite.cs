using UnityEngine;

public class ScrollingSprite : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform otherSprite;

    private float width;

    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        width = sr.bounds.size.x;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (otherSprite != null && transform.position.x <= otherSprite.position.x - width)
        {
            transform.position = new Vector3(
                otherSprite.position.x + width,
                transform.position.y,
                transform.position.z
            );
        }
    }
}