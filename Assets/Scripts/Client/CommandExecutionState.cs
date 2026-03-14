
public static class CommandExecutionState<T>
{
    public static bool IsDone
    {
        get
        {
            bool value = _isDone;
            _isDone = false;   // 读取后自动重置
            return value;
        }
        set
        {
            _isDone = value;
        }
    }
    private static bool _isDone = false;

    public static bool Success
    {
        get
        {
            bool value = _success;
            _success = false;   // 读取后自动重置
            return value;
        }
        set
        {
            _success = value;
        }
    }
    private static bool _success = false;
}
