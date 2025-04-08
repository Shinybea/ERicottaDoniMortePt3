using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System;

//TODO:
// stun status
// differenzia player 1 e player 2



public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private int coyoteTimer = 100;
    [SerializeField] private int jumpBufferTimer = 10;

    private Rigidbody rb;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool wasGrounded;
    [SerializeField] private int speedTracker = 0;
    [SerializeField] private int[] speedbreakpoints;
    [SerializeField] private float[] accelerationValues;
    [SerializeField] private float dashSpeed;
    [SerializeField] private int dashTimer;
    [SerializeField] private bool isPlayer1;
    private float horizontalInput;
    private float verticalInput;
    private bool facingRight = true;
    private Dictionary<string, int> activeTimers = new Dictionary<string, int>();
    [SerializeField] private bool hasJumped;
    private PlayerControls playerControls;
    private Vector2 dashDirection;
    private bool hasGravity = true;
    private bool isDashing = false;
    private object controls; 

    //START & UPDATE
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerControls = new PlayerControls(); // Inizializza i controlli
        playerControls.Enable(); // Abilita gli input

        rb.freezeRotation = true;

        activeTimers.Add("coyoteTime", 0);
        activeTimers.Add("jumpBuffer", 0);
        activeTimers.Add("dashTimer", 0);



    }

    void Update()
    {

        UpdateTimers();

        JumpControl();

        HandleMovement();

        rb.useGravity = false;
        if (hasGravity) rb.AddForce(AdjustGravity());


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
        HandlePlayerCollision(collision);
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
        bool jumpPressed = isPlayer1?playerControls.Player1.Jump.triggered:playerControls.Player2.Jump.triggered;


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
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, AdjustSpeed(jumpForce), rb.linearVelocity.z);
    }

    void HandleMovement()
    {
        if (speedTracker <= 0)
        {
            speedTracker = 0;
        }
        float speed;

        Vector2 movementInput = isPlayer1?playerControls.Player1.Move.ReadValue<Vector2>():playerControls.Player2.Move.ReadValue<Vector2>();
        horizontalInput = movementInput.x;
        Vector3 moveDirection = new Vector3(horizontalInput, 0, 0);

        if (!DashController(movementInput))
        {
            speed = AdjustSpeed(moveDirection.x) * moveSpeed;
            speedTracker++;

            if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
            {
                Flip();
                speedTracker -= 10;
            }
            // Apply different movement force based on ground state
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

    void HandlePlayerCollision(Collision collision){
        if (collision.gameObject.CompareTag("Player")){
            Debug.Log("collided");
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

    bool DashController(Vector2 movementInput)
    {
    
        if (isPlayer1?playerControls.Player1.Dash.triggered:playerControls.Player2.Dash.triggered)
        {
            activeTimers["dashTimer"] = dashTimer;
            dashDirection = movementInput;


        }

        if (activeTimers["dashTimer"] > 0)
        {
            isDashing = true;
            rb.linearVelocity = new Vector3(
                moveSpeed * dashDirection.x * dashSpeed,
                moveSpeed * dashDirection.y * dashSpeed,
                0
            );
            Debug.Log("dashing");
            speedTracker++;
            return true;
        }

        if(activeTimers["dashTimer"] == 0&&isDashing){
            isDashing = false;
            rb.linearVelocity = new Vector3(0,0,0);
        }
        return false;

    }
}