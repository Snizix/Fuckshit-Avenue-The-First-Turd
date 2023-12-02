using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        //Debug.Log(GetNameOfCurrentState());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        _ctx.Animator.SetBool(_ctx.IsWalkingHash, false);
        _ctx.Animator.SetBool(_ctx.IsRunningHash, false);
        _ctx.CurrentMovementX = 0;
        _ctx.CurrentMovementZ = 0;
    }

    public override void ExitState()
    {
       
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsMovementPressed && _ctx.IsRunPressed)
        {
            SetSubState(_factory.Run());
        }
        else if (_ctx.IsMovementPressed)
        {
            SetSubState(_factory.Walk());
        }
    }

    public override void InitializeSubStates()
    {
        
    }

    public override string GetNameOfCurrentState()
    {
        return "Idle State";
    }
}
