
public class ChipStateMachine
{
    public ChipState currentState;
    public void Initialize(ChipState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void ChangeState(ChipState state)
    {
        if (currentState != state)
        {
            currentState.Exit();
            currentState = state;
            currentState.Enter();
        }
    }
}
