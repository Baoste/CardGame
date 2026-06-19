using Game.Domain;
using System.Collections.Generic;
using System;
using System.Collections;

public static class ProcessDispatcher
{
    public delegate IEnumerator DelegateFuncWithParams(object[] parameters);

    public static readonly Dictionary<string, DelegateFuncWithParams> map = new();

    public static void Register(string type, DelegateFuncWithParams action)
    {
        if (!map.TryAdd(type, action))
            map[type] = action;
            // throw new InvalidOperationException($"Handler already registered for type: {type}");
    }

    public static IEnumerator Process(string type, object[] parameters)
    {
        if (map.TryGetValue(type, out var action))
        {
            yield return action(parameters);
        }

        yield break;
    }
}