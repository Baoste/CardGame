using System.Collections.Generic;
using System;

namespace Game.Domain
{
    public static class NetEventRegistry
    {
        private static Dictionary<string, System.Type> _typeMap
            = new Dictionary<string, System.Type>();

        public static void Register<T>(string typeName)
            where T : INetEventPayload
        {
            _typeMap[typeName] = typeof(T);
        }

        public static System.Type GetPayloadType(string typeName)
        {
            return _typeMap.TryGetValue(typeName, out var t) ? t : null;
        }
    }

    // ===== Event (Server -> Client) =====
    [Serializable]
    public struct NetEvent
    {
        public int Index;
        public string Type;
        public string Payload;  // Json
    }

    [Serializable]
    public struct Snapshot
    {
        public string matchId;
        public int slot;              // 你在本局的座位：0 或 1
        public int serverLastEventIndex; // 服务器当前最后事件 index（可能为 -1）
    }
}
