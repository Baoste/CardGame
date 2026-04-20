using DG.Tweening;
using FishNet.Demo.AdditiveScenes;
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

        skillCard.RemoveFromHand();
        skillCard.MoveToExecutePosition();

        ExecuteCardView executeCardView = skillCard.isOpponent ? SceneViewManager.opponentExecuteCardView : SceneViewManager.myExecuteCardView;
        executeCardView.DestroyCard(skillCard.gameObject);
    }

    public override void Exit()
    {
        base.Exit();
        ClientEffectContext.isExecutingSkillCard = false;
    }

    public override void OnMouseEnter()
    {
        skillCard.instance.ShowInfo();
    }

    public override void OnMouseExit()
    {
        skillCard.instance.HideInfo();
    }
}
