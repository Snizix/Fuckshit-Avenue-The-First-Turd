using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
        InitializeSubStates();
    }

    bool wasHoldingJump;

    public override void EnterState()
    {
        if (_ctx.IsJumpPressed)
            wasHoldingJump = true;
    }

    public override void UpdateState()
    {
        if (wasHoldingJump && !_ctx.IsJumpPressed)
            wasHoldingJump = false;
        SetGroundedGravity();
        CheckSwitchStates();
    }

    public override void ExitState() {}

    public override void CheckSwitchStates()
    {
        if(_ctx.IsJumpPressed && _ctx.CharacterController.isGrounded && !wasHoldingJump)
        {
            SwitchStates(_factory.Jump());
        }
    }

    public override void InitializeSubStates() 
    {
        if(!_ctx.IsMovementPressed && !_ctx.IsRunPressed)
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

    void SetGroundedGravity()
    {
        _ctx.CurrentMovementY = _ctx.Gravity;
    }

    public override string GetNameOfCurrentState()
    {
        return "Grounded State";
    }
}
