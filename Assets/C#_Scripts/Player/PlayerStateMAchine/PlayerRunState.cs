using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        //Debug.Log(GetNameOfCurrentState());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        _ctx.Animator.SetBool(_ctx.IsWalkingHash, false);
        _ctx.Animator.SetBool(_ctx.IsRunningHash, true);
        _ctx.CurrentMovementX = _ctx.CurrentMovementInput.x * _ctx.RunMultiplier;
        _ctx.CurrentMovementZ = _ctx.CurrentMovementInput.y * _ctx.RunMultiplier;
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
        else if (_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.Walk());
        }
    }

    public override void InitializeSubStates()
    {
        
    }

    public override string GetNameOfCurrentState()
    {
        return "Run State";
    }
}
