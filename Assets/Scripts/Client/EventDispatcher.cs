using Game.Domain;
using System;
using System.Collections.Generic;

public interface IEventHandler
{
    // 处理并返回事件（或 default 表示无事件）
    bool Handle(NetEvent ev);
}


public class EventHandler
{
}

public sealed class EventDispatcher
{
    public readonly Dictionary<string, IEventHandler> map = new();

    public EventDispatcher Register(string type, IEventHandler handler)
    {
        if (!map.TryAdd(type, handler))
            throw new InvalidOperationException($"Handler already registered for type: {type}");
        return this;
    }

    public bool Process(NetEvent ev)
    {
        if (map.TryGetValue(ev.type, out var handler))
            return handler.Handle(ev);

        return false;
    }
}
