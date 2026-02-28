using Game.Domain;
using System;
using System.Collections.Generic;

public interface ICommandHandler
{
    // 处理并返回事件（或 default 表示无事件）
    ResolvedEvent Handle(NetCommand cmd);
}

public static class CommandDispatcher
{
    public static readonly Dictionary<string, ICommandHandler> map = new();

    public static void Register(string type, ICommandHandler handler)
    {
        if (!map.TryAdd(type, handler))
            throw new InvalidOperationException($"Handler already registered for type: {type}");
    }

    public static ResolvedEvent Process(NetCommand cmd)
    {
        if (map.TryGetValue(cmd.type, out var handler))
            return handler.Handle(cmd);

        throw new InvalidOperationException($"No handler registered for command type: {cmd.type}");
    }
}