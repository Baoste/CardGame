using Game.Domain;
using System;
using System.Collections.Generic;

public interface ICommandHandler
{
    // 处理并返回事件（或 default 表示无事件）
    ResolvedEvent Handle(Command cmd);
}

public sealed class CommandDispatcher
{
    public readonly Dictionary<string, ICommandHandler> map = new();

    public CommandDispatcher Register(string type, ICommandHandler handler)
    {
        if (!map.TryAdd(type, handler))
            throw new InvalidOperationException($"Handler already registered for type: {type}");
        return this;
    }

    public ResolvedEvent Process(Command cmd)
    {
        if (map.TryGetValue(cmd.type, out var handler))
            return handler.Handle(cmd);

        return default;
    }
}