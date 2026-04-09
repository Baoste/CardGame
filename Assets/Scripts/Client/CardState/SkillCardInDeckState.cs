using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardInDeckState : SkillCardState
{
    public SkillCardInDeckState(SkillCardStateMachine stateMachine, SkillCard skillCard, string animatorName) : base(stateMachine, skillCard, animatorName)
    {
    }
}
