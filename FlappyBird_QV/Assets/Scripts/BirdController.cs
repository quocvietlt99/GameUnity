using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BirdController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float jumpForce = 5.5f;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private float maxUpAngle = 25f;
    [SerializeField] private float maxDownAngle = -90f;
    [SerializeField] private float rotateSpeed = 8f;

    [Header("Flap Sprites")]
    [SerializeField] private Sprite flapUp;
    [SerializeField] private Sprite flapMid;
    [SerializeField] private Sprite flapDown;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isDead;
    private Coroutine flapCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        rb.gravityScale = gravityScale;
        rb.velocity = Vector2.zero;

        if (flapMid != null)
            sr.sprite = flapMid;
    }

    private void Update()
    {
        if (isDead) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Fly();
            PlayFlapAnimation();
        }

        RotateBird();
    }

    private void Fly()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void RotateBird()
    {
        float angle;

        if (rb.velocity.y > 0)
            angle = maxUpAngle;
        else
            angle = Mathf.Lerp(0f, maxDownAngle, -rb.velocity.y / 5f);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0f, 0f, angle),
            rotateSpeed * Time.deltaTime
        );
    }

    private void PlayFlapAnimation()
    {
        if (flapCoroutine != null)
            StopCoroutine(flapCoroutine);

        flapCoroutine = StartCoroutine(FlapRoutine());
    }

    private IEnumerator FlapRoutine()
    {
        if (flapUp != null) sr.sprite = flapUp;
        yield return new WaitForSeconds(0.05f);

        if (flapMid != null) sr.sprite = flapMid;
        yield return new WaitForSeconds(0.05f);

        if (flapDown != null) sr.sprite = flapDown;
        yield return new WaitForSeconds(0.05f);

        if (flapMid != null) sr.sprite = flapMid;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pipe"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("ScoreZone"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.simulated = false;

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}