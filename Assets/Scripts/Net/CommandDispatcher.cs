using Game.Domain;
using Game.Server;
using System;
using System.Collections.Generic;

public interface ICommandHandler
{
    // 处理并返回事件（或 default 表示无事件）
    ResolvedEvent Handle(Command cmd);
}

public sealed class CommandDispatcher
{
    private readonly Dictionary<string, ICommandHandler> _map = new();

    public CommandDispatcher Register(string type, ICommandHandler handler)
    {
        if (!_map.TryAdd(type, handler))
            throw new InvalidOperationException($"Handler already registered for type: {type}");
        return this;
    }

    public ResolvedEvent Process(Command cmd)
    {
        if (_map.TryGetValue(cmd.type, out var handler))
            return handler.Handle(cmd);

        return default;
    }
}