using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private int coyoteTimer = 100;
    [SerializeField] private int jumpBufferTimer = 10;
    [SerializeField] private int[] speedbreakpoints;
    [SerializeField] private float[] accelerationValues;
    [SerializeField] private float dashSpeed;
    [SerializeField] private int dashTimer;
    [SerializeField] private bool isPlayer1;
    [SerializeField] private int baseKnockback;
    [SerializeField] float knockBackForce;

    private Rigidbody rb;
    private PlayerControls playerControls;
    
    // State variables
    private bool isGrounded;
    private bool wasGrounded;
    private int speedTracker = 0;
    private bool hasJumped;
    private bool facingRight = true;
    private bool hasGravity = true;
    private bool isDashing = false;
    
    // Input variables
    private float horizontalInput;
    private float verticalInput;
    private bool jumpPressed;
    private Vector2 movementInput;
    private Vector2 dashDirection;
    
    // Timers
    private Dictionary<string, int> activeTimers = new Dictionary<string, int>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerControls = new PlayerControls();
        playerControls.Enable();
        rb.freezeRotation = true;

        activeTimers.Add("coyoteTime", 0);
        activeTimers.Add("jumpBuffer", 0);
        activeTimers.Add("dashTimer", 0);
        activeTimers.Add("stun", 0);
    }

    void Update()
    {
        // Get input in Update
        movementInput = isPlayer1 ? playerControls.Player1.Move.ReadValue<Vector2>() : playerControls.Player2.Move.ReadValue<Vector2>();
        jumpPressed = isPlayer1 ? playerControls.Player1.Jump.triggered : playerControls.Player2.Jump.triggered;
        
        // Handle timers in Update since they're not physics-based
        UpdateTimers();
        
        // Handle jump input and buffering in Update
        JumpControl();
        
        // Check for dash input
        if (isPlayer1 ? playerControls.Player1.Dash.triggered : playerControls.Player2.Dash.triggered)
        {
            activeTimers["dashTimer"] = dashTimer;
            dashDirection = movementInput;
        }
    }

    void FixedUpdate()
    {
        // Physics-based movement in FixedUpdate
        if (activeTimers["stun"] == 0)
        {
            HandleMovement();
        }

        
        // Apply gravity and knockback forces
        knockBackForce = AdjustSpeed(baseKnockback);
        rb.useGravity = false;
        if (hasGravity) rb.AddForce(AdjustGravity());
    }

    void Bounce() {
        if (activeTimers["stun"] > 0)
        {
            
        }
    }

    void OnCollisionStay(Collision collision)
    {
        CheckGround(collision);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("onCollisionEnter");        
        CheckGround(collision);
        HandlePlayerCollision(collision);

    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    void JumpControl()
    {
        if (!wasGrounded && isGrounded)
        {
            hasJumped = false;
        }

        if (jumpPressed)
        {
            if ((isGrounded || activeTimers["coyoteTime"] > 0) && !hasJumped)
            {
                ExecuteJump();
            }
            else
            {
                activeTimers["jumpBuffer"] = jumpBufferTimer;
            }
        }
        else
        {
            if (isGrounded && !wasGrounded && activeTimers["jumpBuffer"] > 0)
            {
                ExecuteJump();
            }
        }

        if (!isGrounded && wasGrounded && !hasJumped)
        {
            activeTimers["coyoteTime"] = coyoteTimer;
        }

        wasGrounded = isGrounded;
    }

    void ExecuteJump()
    {
        activeTimers["coyoteTime"] = 0;
        activeTimers["jumpBuffer"] = 0;
        hasJumped = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, AdjustSpeed(jumpForce), rb.linearVelocity.z);
    }

    void HandleMovement()
    {
        if (speedTracker <= 0)
        {
            speedTracker = 0;
        }

        horizontalInput = movementInput.x;
        Vector3 moveDirection = new Vector3(horizontalInput, 0, 0);

        if (!DashController())
        {
            float speed = AdjustSpeed(moveDirection.x) * moveSpeed;
            speedTracker++;

            if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
            {
                Flip();
                speedTracker -= 10;
            }

            rb.linearVelocity = new Vector3(
                speed,
                rb.linearVelocity.y,
                0
            );

            if (horizontalInput == 0 && rb.linearVelocity.y == 0)
            {
                speedTracker -= 10;
            }
        }
    }

    void CheckGround(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5)
            {
                isGrounded = true;
            }
        }
    }

    void HandlePlayerCollision(Collision collision)
    {
        Vector2 _lastCollisionDirection;
        if (collision.gameObject.CompareTag("Player")|| activeTimers["stun"]>0)
        {
            Debug.Log("collided. collision-->"+collision.ToString());
            Vector2 direction = collision.transform.position - transform.position;
            _lastCollisionDirection = direction.normalized;

            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (collision.gameObject.CompareTag("Player"))
            {
                ApplyKnockback(_lastCollisionDirection, player.knockBackForce);
            }
            speedTracker = 0;
        }

    }

    void UpdateTimers()
    {
        List<string> keysToUpdate = new List<string>(activeTimers.Keys);

        foreach (string key in keysToUpdate)
        {
            if (activeTimers[key] > 0)
            {
                activeTimers[key] -= 1;
            }
        }
    }

    float AdjustSpeed(float speed)
    {
        if (speedTracker >= speedbreakpoints[2])
        {
            speed *= accelerationValues[2];
        }
        else if (speedTracker >= speedbreakpoints[1])
        {
            speed *= accelerationValues[1];
        }
        else if (speedTracker >= speedbreakpoints[0])
        {
            speed *= accelerationValues[0];
        }
        return speed;
    }

    Vector3 AdjustGravity()
    {
        var gravity = Physics.gravity;
        if (speedTracker >= speedbreakpoints[2])
        {
            gravity *= accelerationValues[2];
        }
        else if (speedTracker >= speedbreakpoints[1])
        {
            gravity *= accelerationValues[1];
        }
        else if (speedTracker >= speedbreakpoints[0])
        {
            gravity *= accelerationValues[0];
        }
        return gravity;
    }

    bool DashController()
    {
        if (activeTimers["dashTimer"] > 0)
        {
            isDashing = true;
            rb.linearVelocity = new Vector3(
                moveSpeed * dashDirection.x * dashSpeed,
                moveSpeed * dashDirection.y * dashSpeed,
                0
            );
            speedTracker++;
            return true;
        }

        if (activeTimers["dashTimer"] == 0 && isDashing)
        {
            isDashing = false;
            rb.linearVelocity = new Vector3(0, 0, 0);
        }
        return false;
    }

    void ApplyKnockback(Vector2 direction, float force)
    {
        activeTimers["stun"] = 100;
        rb.linearVelocity = new Vector3(
                -direction.x * force,
                -direction.y * force,
                0
            );
    }
}