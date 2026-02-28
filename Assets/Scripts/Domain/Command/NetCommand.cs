using System.Collections.Generic;
using System;

namespace Game.Domain
{
    public static class CommandRegistry
    {
        private static Dictionary<string, System.Type> _typeMap
            = new Dictionary<string, System.Type>();

        public static void Register<T>(string typeName)
            where T : ICommand
        {
            _typeMap[typeName] = typeof(T);
        }

        public static System.Type GetPayloadType(string typeName)
        {
            return _typeMap.TryGetValue(typeName, out var t) ? t : null;
        }
    }

    // ===== Command (Client -> Server) =====
    [Serializable]
    public struct Command
    {
        public int index;
        public string type;
        public string jsonData;  // Json
    }
}
