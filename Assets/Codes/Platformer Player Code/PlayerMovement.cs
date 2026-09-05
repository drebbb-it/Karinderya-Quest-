using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 60f; 
    public float deacceleration = 60f;

    [Header("Jumping")]
    public float jumpForce = 1f;
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
    private float moveInput;
    private bool isGrounded;
    private bool jumpHeld;
    private float coyoteTimer;
    private float jumpBufferTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
        Debug.Log("Is Grounded: " + isGrounded);
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // jump trigger 
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            Jump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
        if(moveInput != 0)
        {
            // Flip the player sprite based on movement direction
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1f, 1f);
        }

    }
    void FixedUpdate()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDifference = targetSpeed - rb.linearVelocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deacceleration;
        float movement = speedDifference * accelerationRate * Time.fixedDeltaTime;
        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement, rb.linearVelocity.y);

        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallingGravityMultiplier;
        }
        else if(rb.linearVelocity.y > 0 && !jumpHeld)
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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x,jumpForce);   
    }
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null ) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
