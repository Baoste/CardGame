
public class PointCardStateMachine
{
    public PointCardState currentState;
    public void Initialize(PointCardState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void ChangeState(PointCardState state)
    {
        if (currentState != state)
        {
            currentState.Exit();
            currentState = state;
            currentState.Enter();
        }
    }
}
