using System.Collections;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;

    [Header("Wing Animation")]
    public Sprite wingUpSprite;
    public Sprite wingMiddleSprite;
    public Sprite wingDownSprite;
    public float flapFrameTime = 0.08f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;
    private bool canControl = false;

    private Coroutine flapCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.simulated = false;

        if (wingMiddleSprite != null)
        {
            spriteRenderer.sprite = wingMiddleSprite;
        }
    }

    void Update()
    {
        if (!canControl || isDead) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0)
        {
            Jump();
        }

        float angle = Mathf.Clamp(rb.velocity.y * 5f, -35f, 35f);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void StartBird()
    {
        rb.simulated = true;
        canControl = true;
        isDead = false;

        Jump();
    }

    void Jump()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        PlayFlapAnimation();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayWing();
        }
    }

    void PlayFlapAnimation()
    {
        if (flapCoroutine != null)
        {
            StopCoroutine(flapCoroutine);
        }

        flapCoroutine = StartCoroutine(FlapRoutine());
    }

    IEnumerator FlapRoutine()
    {
        if (wingUpSprite != null)
        {
            spriteRenderer.sprite = wingUpSprite;
        }

        yield return new WaitForSeconds(flapFrameTime);

        if (wingMiddleSprite != null)
        {
            spriteRenderer.sprite = wingMiddleSprite;
        }

        yield return new WaitForSeconds(flapFrameTime);

        if (wingDownSprite != null)
        {
            spriteRenderer.sprite = wingDownSprite;
        }

        yield return new WaitForSeconds(flapFrameTime);

        if (wingMiddleSprite != null)
        {
            spriteRenderer.sprite = wingMiddleSprite;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pipe"))
        {
            isDead = true;
            canControl = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}