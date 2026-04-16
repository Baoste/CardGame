using DG.Tweening;
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
        skillCard.instance.ShowInfo();

        ExecuteCardView executeCardView = skillCard.isOpponent ? SceneViewManager.opponentExecuteCardView : SceneViewManager.myExecuteCardView;
        executeCardView.DestroyCard(skillCard.gameObject);
    }

    public override void Exit()
    {
        base.Exit();
        ClientEffectContext.isExecutingSkillCard = false;
    }
}
