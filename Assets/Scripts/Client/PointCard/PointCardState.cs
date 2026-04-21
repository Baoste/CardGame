
public class PointCardState
{
    public PointCardStateMachine stateMachine;
    public PointCardController pointCard;
    public string animatorName;

    public PointCardState(PointCardStateMachine stateMachine, PointCardController pointCard, string animatorName)
    {
        this.stateMachine = stateMachine;
        this.pointCard = pointCard;
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

    public virtual void OnMouseEnter()
    {
    }

    public virtual void OnMouseExit()
    {
    }

    public virtual void OnMouseDown()
    {
    }

    public virtual void OnMouseDrag()
    {
    }

    public virtual void OnMouseUp()
    {
    }
}
