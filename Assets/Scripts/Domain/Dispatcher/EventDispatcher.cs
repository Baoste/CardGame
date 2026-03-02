using Game.Domain;
using System;
using System.Collections.Generic;

public interface IEventHandler
{
    // 处理并返回事件（或 default 表示无事件）
    bool Handle(NetEvent ev);
}


public interface IEventProcess
{
    void Process(object[] objects);
}

public static class EventDispatcher
{
    public static readonly Dictionary<string, IEventHandler> map = new();

    public static void Register(string type, IEventHandler handler)
    {
        if (!map.TryAdd(type, handler))
            throw new InvalidOperationException($"Handler already registered for type: {type}");
    }

    public static bool Process(NetEvent ev)
    {
        if (map.TryGetValue(ev.type, out var handler))
            return handler.Handle(ev);

        return false;
    }
}
