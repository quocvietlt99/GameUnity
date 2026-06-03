using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoScript_Finished : MonoBehaviour
{
    [Header("Jump Settings")]
    public float normalJumpForce = 7f;
    public float highJumpForce = 11f;

    [Header("Double Press Settings")]
    [Tooltip("Khoảng thời gian tối đa giữa 2 lần bấm để tính là nhảy cao.")]
    public float doublePressTime = 0.25f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private float lastJumpPressTime = -10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGrounded();
        HandleJumpInput();
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

        bool pressedJump =
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetMouseButtonDown(0);

        if (!pressedJump)
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

    private void Jump(float jumpForce)
    {
        if (rb == null)
        {
            return;
        }

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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