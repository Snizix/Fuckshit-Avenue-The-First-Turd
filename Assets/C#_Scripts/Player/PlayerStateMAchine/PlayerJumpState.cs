using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) 
    {
        _isRootState = true;
        InitializeSubStates();
    }

    bool isFalling;

    public override void EnterState()
    {
        StartJump();
    }

    public override void UpdateState()
    {
        UpdateGravity();
        AnimateJump();
        CheckSwitchStates();
    }

    public override void ExitState()
    { 
        _ctx.Animator.SetBool(_ctx.IsJumpingHash, false);
        _ctx.Animator.SetBool(_ctx.IsFallingHash, false);
    }

    public override void CheckSwitchStates()
    {
         if (_ctx.CharacterController.isGrounded)
            SwitchStates(_factory.Grounded());
    }

    public override void InitializeSubStates()
    {
        if (!_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.Idle());
        }
        else if (_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.Walk());
        }
        else
        {
            SetSubState(_factory.Run());
        }
    }

    void UpdateGravity()
    {
        isFalling = _ctx.CurrentMovementY <= 0.0f || !_ctx.IsJumpPressed;

        if (isFalling)
        {
            _ctx.CurrentMovementY += _ctx.Gravity * _ctx.FallMultiplier * Time.deltaTime;
        }
        else
        {
            _ctx.CurrentMovementY += _ctx.Gravity * Time.deltaTime;
        }
        
    }

    void StartJump()
    {
        _ctx.CurrentMovementY = _ctx.InitialJumpVelocity;
        // Set normal gravity
    }

    public override string GetNameOfCurrentState()
    {
        return "Jump State";
    }

    void AnimateJump()
    { 
        if(isFalling && _ctx.CurrentMovementY <= 0)
        {
            _ctx.Animator.SetBool(_ctx.IsJumpingHash, false);
            _ctx.Animator.SetBool(_ctx.IsFallingHash, true);
        }
        else
        {
            _ctx.Animator.SetBool(_ctx.IsJumpingHash, true);
            _ctx.Animator.SetBool(_ctx.IsFallingHash, false);
        }
    }
}
