
public class SkillCardStateMachine
{
    public SkillCardState currentState;
    public void Initialize(SkillCardState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void ChangeState(SkillCardState state)
    {
        if (currentState != state)
        {
            currentState.Exit();
            currentState = state;
            currentState.Enter();
        }
    }
}
