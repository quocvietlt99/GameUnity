using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoScript_Finished : MonoBehaviour
{
    [Header("Jump Settings")]
    public float normalJumpForce = 7f;
    public float highJumpForce = 11f;

    [Header("Double Press Settings")]
    [Tooltip("Khoảng thời gian tối đa giữa 2 lần bấm/chạm để tính là nhảy cao.")]
    public float doublePressTime = 0.25f;

    [Header("Gravity Settings")]
    [Tooltip("Gravity khi Dino đang bay lên.")]
    public float upwardGravityScale = 2.2f;

    [Tooltip("Gravity khi Dino đang rơi xuống. Số càng thấp thì rơi càng chậm.")]
    public float fallingGravityScale = 1.0f;

    [Tooltip("Gravity khi Dino đang đứng dưới đất.")]
    public float groundedGravityScale = 2.2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip deathSound;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool isDead;
    private float lastJumpPressTime = -10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (rb != null)
        {
            rb.gravityScale = groundedGravityScale;
        }
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        CheckGrounded();
        HandleJumpInput();
        UpdateGravity();
        UpdateAnimation();
    }

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            isGrounded = true;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void HandleJumpInput()
    {
        if (GameManagerScript_Finished.instance == null)
        {
            return;
        }

        if (!GameManagerScript_Finished.instance.gameRunning)
        {
            return;
        }

        if (!isGrounded)
        {
            return;
        }

        if (!PressedJumpInput())
        {
            return;
        }

        bool isDoublePress = Time.time - lastJumpPressTime <= doublePressTime;
        lastJumpPressTime = Time.time;

        if (isDoublePress)
        {
            Jump(highJumpForce);
        }
        else
        {
            Jump(normalJumpForce);
        }
    }

    private bool PressedJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            return true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                return true;
            }
        }

        return false;
    }

    private void Jump(float jumpForce)
    {
        if (rb == null)
        {
            return;
        }

        rb.gravityScale = upwardGravityScale;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        PlaySound(jumpSound);
    }

    private void UpdateGravity()
    {
        if (rb == null)
        {
            return;
        }

        if (isGrounded)
        {
            rb.gravityScale = groundedGravityScale;
            return;
        }

        if (rb.velocity.y > 0f)
        {
            rb.gravityScale = upwardGravityScale;
        }
        else if (rb.velocity.y < 0f)
        {
            rb.gravityScale = fallingGravityScale;
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null)
        {
            return;
        }

        if (HasAnimatorParameter("IsGrounded"))
        {
            animator.SetBool("IsGrounded", isGrounded);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckObstacleHit(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckObstacleHit(collision.collider);
    }

    private void CheckObstacleHit(Collider2D collision)
    {
        if (isDead)
        {
            return;
        }

        if (collision == null)
        {
            return;
        }

        if (collision.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        PlaySound(deathSound);

        if (GameManagerScript_Finished.instance != null)
        {
            GameManagerScript_Finished.instance.GameOver();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null)
        {
            return;
        }

        if (clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    private bool HasAnimatorParameter(string parameterName)
    {
        if (animator == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}