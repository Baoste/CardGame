using Game.Domain;

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
