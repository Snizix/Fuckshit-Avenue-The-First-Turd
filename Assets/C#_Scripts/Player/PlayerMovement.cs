using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Player_Input playerInput;
    public CharacterController characterController { get; private set; }

    Vector2 currentMovementInput;
    public Vector3 currentMovement;
    public Vector3 appliedMovement;
    public bool isMovementPressed { get; private set; }

    public float walkSpeed;
    public float rotationSpeed;
    public bool useDefaultHorizontalMovement;
    public bool useDefaultRotation;

    float gravity = -9.8f;
    float groundedGravity = -0.05f;

    bool isJumpPressed;
    public bool isJumping;
    float initialJumpSpeed;
    public float maxJumpHeight = 1;
    public float maxJumpTime = 0.5f;
    public float fallMultiplier = 2;
    public float maxFallSpeed = -20f;

    private void Awake()
    {
        useDefaultHorizontalMovement = true;
        useDefaultRotation = true;
        characterController = GetComponent<CharacterController>();

        playerInput = new Player_Input();
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;
        playerInput.CharacterControls.Jump.started += OnJumpInput;
        playerInput.CharacterControls.Jump.canceled += OnJumpInput;

        SetupJumpVars();
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void SetupJumpVars()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpSpeed = (2 * maxJumpHeight) / timeToApex;
    }


    void OnJumpInput(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    // Update is called once per frame
    void Update()
    {
        if(useDefaultHorizontalMovement)
        {
            Vector3 movementVector = new Vector3(currentMovement.x * walkSpeed, appliedMovement.y, currentMovement.z * walkSpeed);
            characterController.Move(movementVector * Time.deltaTime);
        }

        if(useDefaultRotation)
            RotateObject();

        ApplyGravity();
        HandleJump();
    }

    void HandleJump()
    {
        if(!isJumping && characterController.isGrounded && isJumpPressed)
        {
            isJumping = true;
            appliedMovement.y = initialJumpSpeed;
        }
        else if(isJumping && characterController.isGrounded && !isJumpPressed)
        {
            isJumping = false;
        }
    }

    void ApplyGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
       
        if(characterController.isGrounded)
        {
            appliedMovement.y = groundedGravity;
        }
        else if(isFalling)
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + currentMovement.y) * 0.5f;

            appliedMovement.y = Mathf.Max(nextYVelocity, maxFallSpeed);
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + currentMovement.y) * 0.5f;

            appliedMovement.y = nextYVelocity;
        }
    }

    void RotateObject()
    {
        Vector3 positionToLookAt = new Vector3(currentMovement.x, 0, currentMovement.z);
        Quaternion currentRotation = transform.rotation;
        if(isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
