using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 60f;
    public float deacceleration = 60f;

    [Header("Jumping")]
    public float jumpForce = 16f;
    public float gravityScale = 10f;
    public float fallingGravityMultiplier = 1.8f; // faster falling
    public float lowJumpMultiplier = 2.5f; // short hop

    [Header("Ground Check")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask Ground;

    [Header("Feel / Forgiveness")]
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    private Rigidbody2D rb;
    private Animator anim; // Added Animator reference
    private float moveInput;
    private bool isGrounded;
    private bool jumpHeld;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool isJumpRequested;
    private Vector3 originalScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Cache Animator component
        rb.gravityScale = gravityScale;

        originalScale = transform.localScale;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
        jumpHeld = Input.GetButton("Jump");

        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, Ground);

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // Jump trigger 
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            isJumpRequested = true;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        if (moveInput != 0)
        {
            // Flip the player sprite based on movement direction
            transform.localScale = new Vector3(Mathf.Sign(moveInput) * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        // Update Animator Parameters
        UpdateAnimationState();
    }

    void FixedUpdate()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDifference = targetSpeed - rb.linearVelocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deacceleration;
        float movement = speedDifference * accelerationRate * Time.fixedDeltaTime;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement, rb.linearVelocity.y);

        if (isJumpRequested)
        {
            Jump();
            isJumpRequested = false;
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallingGravityMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            rb.gravityScale = gravityScale * lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void UpdateAnimationState()
    {
        // Pass essential variables to the Animator Controller
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetFloat("xAbsSpeed", Mathf.Abs(rb.linearVelocity.x));
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}