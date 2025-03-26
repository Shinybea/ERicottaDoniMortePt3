using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using JetBrains.Annotations;

//TODO:
// accellerazione
// stun status
// dash


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private int coyoteTimer = 100;
    [SerializeField] private int jumpBufferTimer = 10;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool wasGrounded;
    [SerializeField] private int speedTracker = 0;
    [SerializeField] private int[] speedbreakpoints;
    [SerializeField] private float[] accelerationValues;
    private float horizontalInput;
    private float verticalInput;
    private bool facingRight = true;
    private Dictionary<string, int> activeTimers = new Dictionary<string, int>();
    [SerializeField] private bool hasJumped;
    private PlayerControls playerControls;

    //START & UPDATE
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerControls = new PlayerControls(); // Inizializza i controlli
        playerControls.Enable(); // Abilita gli input

        rb.freezeRotation = true;

        activeTimers.Add("coyoteTime", 0);
        activeTimers.Add("jumpBuffer", 0);


    }
    void Update()
    {

        UpdateTimers();

        JumpControl();

        HandleMovement();

    }

    //COLLISION CONTROL
    void OnCollisionStay(Collision collision)
    {
        CheckGround(collision);
        Debug.Log("collided");
    }

    void OnCollisionEnter(Collision collision)
    {
        CheckGround(collision);
        Debug.Log("collided");
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    //MOVEMENT
    void Flip()
    {
        // Switch facing direction
        facingRight = !facingRight;

        // Rotate 180 degrees around Y axis
        transform.Rotate(0, 180, 0);
    }

    void JumpControl()
    {

        //if (isGrounded) hasJumped = false;
        if (!wasGrounded && isGrounded)
        {
            hasJumped = false;
        }
        bool jumpPressed = playerControls.Player1.Jump.triggered;


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
            Debug.Log("coyoteStart: hasJumped-->" + hasJumped + "//isGrounded-->" + isGrounded + "//wasGrounded-->" + wasGrounded);
            activeTimers["coyoteTime"] = coyoteTimer;
        }

        wasGrounded = isGrounded;
    }

    void ExecuteJump()
    {
        activeTimers["coyoteTime"] = 0;
        activeTimers["jumpBuffer"] = 0;
        hasJumped = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    void HandleMovement()
    {
        if(speedTracker<=0){
            speedTracker=0;
        }

        Vector2 movementInput = playerControls.Player1.Move.ReadValue<Vector2>();
        horizontalInput = movementInput.x;
        speedTracker++;

        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
        {
            Flip();
            speedTracker-=10;
        }

        // Calculate movement force
        Vector3 moveDirection = new Vector3(horizontalInput, 0, 0);

        float speed = AdjustSpeed(moveDirection.x) * moveSpeed;

        // Apply different movement force based on ground state
        rb.linearVelocity = new Vector3(
            speed,
            rb.linearVelocity.y,
            0
        );

        if(horizontalInput == 0){
            speedTracker-=10;
        }
    }
    //MISC
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

    float AdjustSpeed(float speed){
        if(speedTracker>=speedbreakpoints[2]){
            speed*=accelerationValues[2];
        }else if(speedTracker>=speedbreakpoints[1]){
            speed*=accelerationValues[1];
        }else if(speedTracker>=speedbreakpoints[0]){
            speed*=accelerationValues[0];
        }
        return speed;
    }
}