using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipDiscardState : ChipState
{
    public ChipDiscardState(ChipStateMachine stateMachine, ChipController chip, string animatorName) : base(stateMachine, chip, animatorName)
    {
    }
    public override void Enter()
    {
        // base.Enter();

        chip.rb.useGravity = false;
        chip.col.isTrigger = true;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
