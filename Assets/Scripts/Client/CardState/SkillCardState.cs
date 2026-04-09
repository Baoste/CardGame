
public class SkillCardState
{
    public SkillCardStateMachine stateMachine;
    public SkillCardController skillCard;
    public string animatorName;

    public SkillCardState(SkillCardStateMachine stateMachine, SkillCardController skillCard, string animatorName)
    {
        this.stateMachine = stateMachine;
        this.skillCard = skillCard;
        this.animatorName = animatorName;
    }

    public virtual void Enter()
    {
        // skillCard.animator.SetBool(animatorName, true);
    }


    public virtual void Exit()
    {
        // skillCard.animator.SetBool(animatorName, false);
    }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    public virtual void OnMouseEnter() { }
    public virtual void OnMouseExit() { }
    public virtual void OnMouseDown() { }
    public virtual void OnMouseDrag() { }
    public virtual void OnMouseUp() { }
}
