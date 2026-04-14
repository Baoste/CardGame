using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipPlacedState : ChipState
{
    public ChipPlacedState(ChipStateMachine stateMachine, ChipController chip, string animatorName) : base(stateMachine, chip, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        chip.rb.useGravity = true;
        chip.col.isTrigger = false;
        chip.rb.AddForceAtPosition(-chip.transform.up * 0.5f, chip.transform.position - chip.transform.right * 0.1f, ForceMode.Impulse);
    }

    public override void Exit()
    {
        base.Exit();
    }

}
