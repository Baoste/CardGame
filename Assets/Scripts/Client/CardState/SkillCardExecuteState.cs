using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardExecuteState : SkillCardState
{
    public SkillCardExecuteState(SkillCardStateMachine stateMachine, SkillCardController skillCard, string animatorName) : base(stateMachine, skillCard, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ClientEffectContext.isExecutingSkillCard = true;
        skillCard.MoveToExecutePosition();
    }

    public override void Exit()
    {
        base.Exit();
        ClientEffectContext.isExecutingSkillCard = false;
    }
}
