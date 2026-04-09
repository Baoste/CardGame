using Game.Domain;
using Newtonsoft.Json;

public class SkillCardReadyFallState : SkillCardState
{
    public SkillCardReadyFallState(SkillCardStateMachine stateMachine, SkillCard skillCard, string animatorName) : base(stateMachine, skillCard, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        skillCard.MoveToFallPosition();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
