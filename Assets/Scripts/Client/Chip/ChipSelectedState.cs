using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipSelectedState : ChipState
{
    public ChipSelectedState(ChipStateMachine stateMachine, ChipController chip, string animatorName) : base(stateMachine, chip, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        chip.outlineControl.OutlineColor = Color.white;
        chip.outlineControl.Enable = 1f;
    }

    public override void Exit()
    {
        base.Exit();
        chip.outlineControl.Enable = 0f;
    }
}
