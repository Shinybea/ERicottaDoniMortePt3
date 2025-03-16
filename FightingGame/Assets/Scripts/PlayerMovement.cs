using UnityEngine;
using System.Collections.Generic;

//TODO:
// quando il player si scontra con la piattaforma di lato, non deve fermarsi
// input buffer
// coyote time
// accellerazione
// stun status
// dash


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private int coyoteTimer = 100;
    [SerializeField] private int inputBufferTimer = 10;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private bool wasGrounded;
    private float horizontalInput;
    private float verticalInput;
    private bool facingRight = true;
    private Dictionary<string, int> activeTimers = new Dictionary<string, int>();
    private bool hasJumped;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        activeTimers.Add("coyoteTime", 0);

    }


    void Update()
    {

        UpdateTimers();

        JumpControl();

        horizontalInput = Input.GetAxis("Horizontal");

        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
        {
            Flip();
        }

        // Calculate movement force
        Vector3 moveDirection = new Vector3(horizontalInput, 0, 0);

        // Apply different movement force based on ground state
        rb.linearVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            rb.linearVelocity.y,
            0
        );


    }

    void Flip()
    {
        // Switch facing direction
        facingRight = !facingRight;

        // Rotate 180 degrees around Y axis
        transform.Rotate(0, 180, 0);
    }


    void UpdateTimers()
    {
        List<string> keysToUpdate = new List<string>(activeTimers.Keys);

        foreach (string key in keysToUpdate)
        {
            if (activeTimers[key] > 0)
            {
                activeTimers[key] -= 1;
                if (activeTimers["coyoteTime"] == 0) Debug.Log("coyote died");
            }

        }
    }

    void JumpControl()
    {
        // Ground check using sphere cast
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
        if (isGrounded) hasJumped = false;


        if (Input.GetButtonDown("Jump") && (isGrounded || activeTimers["coyoteTime"] > 0))
        {
            Debug.Log("jump: hasJumped-->" + hasJumped + "//isGrounded-->" + isGrounded + "//coyote-->" + activeTimers["coyoteTime"]);
            activeTimers["coyoteTime"] = 0;
            hasJumped = true;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        if (!isGrounded && wasGrounded && !hasJumped)
        {
            Debug.Log("coyoteStart: hasJumped-->" + hasJumped + "//isGrounded-->" + isGrounded + "//wasGrounded-->" + wasGrounded);
            activeTimers["coyoteTime"] = coyoteTimer;
        }

        wasGrounded = isGrounded;
    }

}