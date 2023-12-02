using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState() 
    {
        //Debug.Log(GetNameOfCurrentState());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        _ctx.Animator.SetBool(_ctx.IsWalkingHash, true);
        _ctx.Animator.SetBool(_ctx.IsRunningHash, false);
        _ctx.CurrentMovementX = _ctx.CurrentMovementInput.x * _ctx.WalkSpeed;
        _ctx.CurrentMovementZ = _ctx.CurrentMovementInput.y * _ctx.WalkSpeed;
    }

    public override void ExitState() 
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (!_ctx.IsMovementPressed)
        {
            SetSubState(_factory.Idle());
        }
        else if (_ctx.IsMovementPressed && _ctx.IsRunPressed)
        {
            SetSubState(_factory.Run());
        }
    }

    public override void InitializeSubStates()
    {
        
    }

    public override string GetNameOfCurrentState()
    {
        return "Walk State";
    }
}
