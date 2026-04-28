using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Bubble))]
public class BubbleProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;

    private Rigidbody2D rb;
    private Bubble bubble;

    public bool IsFlying { get; private set; }
    public Bubble Bubble => bubble;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bubble = GetComponent<Bubble>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void Launch(Vector2 direction)
    {
        EnsureComponents();

        IsFlying = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = direction.normalized * speed;
    }

    public void AttachToGrid()
    {
        EnsureComponents();

        IsFlying = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    public void FallAway()
    {
        EnsureComponents();

        IsFlying = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1.8f;
        rb.velocity = new Vector2(Random.Range(-1.2f, 1.2f), -2f);
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsFlying) return;

        Collider2D other = collision.collider;

        // Tường thì để vật lý tự nảy
        if (other.CompareTag("Wall"))
            return;

        // Chỉ attach khi chạm trần hoặc bubble khác
        if (other.CompareTag("Ceiling") || other.GetComponent<Bubble>() != null)
        {
            GridManager.Instance.AttachProjectile(this);
            return;
        }

        // Chạm lose line thì game over
        if (other.CompareTag("LoseLine"))
        {
            GameManager.Instance.GameOver();
            return;
        }

        // Còn lại bỏ qua, không được dính xuống dưới
    }

    private void EnsureComponents()
    {
        if (bubble == null)
            bubble = GetComponent<Bubble>();

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }
}