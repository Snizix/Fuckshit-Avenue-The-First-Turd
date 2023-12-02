using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    CharacterController _characterController;
    public Animator _animator;
    Player_Input _playerInput;

    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isRunPressed;

    public float _rotationFactorPerFrame = 15.0f;
    public float _runMultiplier = 4.0f;
    public float _walkSpeed = 2;
    int _zero = 0;

    float _gravity = -9.8f;
    float _groundedGravity = -0.05f;

    bool _isJumpPressed = false;
    float _initialJumpVelocity;
    public float _maxJumpHeight = 4f;
    public float _maxJumpTime = 0.75f;
    public float _fallMultiplier = 2;
    float _maxFallSpeed = -20f;
    bool _isJumping = false;

    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;
    int _isFallingHash;

    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public CharacterController CharacterController { get { return _characterController; } }
    public Animator Animator { get { return _animator; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
    public Vector3 CurrentMovement { get { return _currentMovement; } set { _currentMovement = value; } }
    public Vector3 AppliedMovement { get { return _appliedMovement; } set { _appliedMovement = value; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } set { _initialJumpVelocity = value; } }
    public float WalkSpeed { get { return _walkSpeed; } }
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
    public int IsFallingHash { get { return _isFallingHash; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public bool IsJumping { get { return _isJumping; } set { _isJumping = value; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public float FallMultiplier { get { return _fallMultiplier; } }
    public float RunMultiplier { get { return _runMultiplier; } }
    public float MaxFallSpeed { get { return _maxFallSpeed; } }
    public float CurrentMovementX { get { return _currentMovement.x; } set { _currentMovement.x = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float CurrentMovementZ { get { return _currentMovement.z; } set { _currentMovement.z = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float Gravity { get { return _gravity; } set { _gravity = value; } }
    public float GroundedGravity { get { return _groundedGravity; } set { _groundedGravity = value; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }

    private void Awake()
    {
        _playerInput = new Player_Input();
        _characterController = GetComponent<CharacterController>();

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        // Using a hash will increase performance; idk why though...
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isFallingHash = Animator.StringToHash("isFalling");
       
        _playerInput.CharacterControls.Move.started += OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += OnMovementInput;
        _playerInput.CharacterControls.Move.performed += OnMovementInput;
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;

        SetupJumpVariables();
    }

    private void Update()
    { 
        HandleRotation();
        _currentState.UpdateStates();
        _characterController.Move(_currentMovement * Time.deltaTime);
    }

    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt = new Vector3(_currentMovementInput.x, _zero, _currentMovementInput.y);
        Quaternion currentRotation = transform.rotation;
        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }

    void OnRun(InputAction.CallbackContext context)
    {

        _isRunPressed = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }
}
