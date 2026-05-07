
using Game.Domain;

public class ChipState
{
    public ChipStateMachine stateMachine;
    public ChipController chip;
    public string animatorName;

    public ChipState(ChipStateMachine stateMachine, ChipController chip, string animatorName)
    {
        this.stateMachine = stateMachine;
        this.chip = chip;
        this.animatorName = animatorName;
    }

    public virtual void Enter()
    {
        // skillCard.animator.SetBool(animatorName, true);
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
        {
            stateMachine.ChangeState(chip.inTrayState);
        }
    }


    public virtual void Exit()
    {
        // skillCard.animator.SetBool(animatorName, false);
    }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    public virtual void OnMouseEnter() { }
    public virtual void OnMouseStay() { }
    public virtual void OnMouseExit() { }
    public virtual void OnMouseDown() { }
    public virtual void OnMouseDrag() { }
    public virtual void OnMouseUp() { }
}
