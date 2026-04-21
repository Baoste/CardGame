using Game.Domain;
using Newtonsoft.Json;

public class SkillCardReadyFallState : SkillCardState
{
    public SkillCardReadyFallState(SkillCardStateMachine stateMachine, SkillCardController skillCard, string animatorName) : base(stateMachine, skillCard, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ClientEffectContext.isExecutingSkillCard = true;
        skillCard.MoveToFallPosition();
    }

    public override void Exit()
    {
        base.Exit();
        ClientEffectContext.isExecutingSkillCard = false;
    }
}
