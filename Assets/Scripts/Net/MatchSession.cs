using FishNet.Connection;
using Game.Domain;
using System;
using System.Collections.Generic;

namespace Game.Server   // 这里用你 MatchGateway 所在的 namespace
{
    // ====== Server-only session model ======
    public class PlayerSlot
    {
        public string Token;               // 重连凭证
        public NetworkConnection Conn;     // 在线连接（断线后可能为 null）
        public DateTime LastSeenUtc;       // 用于 GC
    }

    public class MatchSession
    {
        public string MatchId;
        public PlayerSlot[] Slots = { new PlayerSlot(), new PlayerSlot() };

        public readonly List<NetEvent> EventLog = new();
        public readonly List<Command> CmdLog = new();
        public int NextEventIndex = 0;
        public int NextCmdIndex = 0;
        public bool Started;

        public MatchSession(string matchId) => MatchId = matchId;

        public int ServerLastEventIndex => NextEventIndex - 1; // 没事件时为 -1

        public NetEvent AddEvent<T>(string type, T payload)
            where T : INetEventPayload
        {
            var json = UnityEngine.JsonUtility.ToJson(payload);

            var ev = new NetEvent
            {
                Index = NextEventIndex++,
                Type = type,
                Payload = json
            };
            EventLog.Add(ev);
            return ev;
        }

        public NetEvent AddEvent(string type, string jsonData)
        {
            var ev = new NetEvent
            {
                Index = NextEventIndex++,
                Type = type,
                Payload = jsonData
            };
            EventLog.Add(ev);
            return ev;
        }

        public Command AddCommand(string type, string jsonData)
        {
            var cmd = new Command
            {
                index = NextCmdIndex++,
                type = type,
                jsonData = jsonData
            };
            CmdLog.Add(cmd);
            return cmd;
        }
    }
}