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
        public readonly List<NetCommand> CmdLog = new();
        public int NextEventIndex = 0;
        public int NextCmdIndex = 0;
        public bool Started;

        public GameState gameState = new GameState();
        public EffectContext ctx = new EffectContext();
        public Dictionary<int, int> instanceToCardId = new Dictionary<int, int>();

        public MatchSession(string matchId) => MatchId = matchId;

        public int ServerLastEventIndex => NextEventIndex - 1; // 没事件时为 -1

        public NetEvent AddEvent(string type, string jsonData)
        {
            var ev = new NetEvent
            {
                Index = NextEventIndex++,
                type = type,
                jsonData = jsonData
            };
            EventLog.Add(ev);
            return ev;
        }

        public NetCommand AddCommand(string type, string jsonData)
        {
            var cmd = new NetCommand
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