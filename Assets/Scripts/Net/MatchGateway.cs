using System;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;
using Game.Domain;
using FishNet.Demo.AdditiveScenes;

public class MatchGateway : NetworkBehaviour
{
    public static event System.Action<string, int, string, string> OnClientJoined;
    public static event System.Action<Game.Domain.NetEvent> OnClientEvent;
    public static event System.Action<string> OnClientSnapshot;

    // ====== Server-only session model ======
    private class PlayerSlot
    {
        public string Token;               // 重连凭证
        public NetworkConnection Conn;     // 在线连接（断线后可能为 null）
        public DateTime LastSeenUtc;       // 用于 GC
    }

    private class MatchSession
    {
        public string MatchId;
        public PlayerSlot[] Slots = { new PlayerSlot(), new PlayerSlot() };

        public readonly List<NetEvent> EventLog = new();
        public int NextEventIndex = 0;
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
    }

    // Dedicated 多局：matchId -> session
    private readonly Dictionary<string, MatchSession> _sessions = new();

    // 当前连接在哪一局：conn.ClientId -> (matchId, slot)
    private readonly Dictionary<int, (string matchId, int slot)> _connMap = new();

    // 两人都离线后保留多久再回收
    private static readonly TimeSpan KeepAlive = TimeSpan.FromMinutes(5);
    private float _gcTimer;

    // =======================
    // Client -> Server: Join or Create
    // =======================
    [ServerRpc(RequireOwnership = false)]
    public void JoinOrCreateServerRpc(string matchIdOrEmpty, NetworkConnection sender = null)
    {
        if (sender == null) return;

        // 如果这个连接已经在某局里，先踢掉旧映射（最小实现：直接覆盖）
        if (_connMap.TryGetValue(sender.ClientId, out var old))
        {
            DetachConnection(sender.ClientId, old.matchId, old.slot);
        }

        var matchId = string.IsNullOrWhiteSpace(matchIdOrEmpty) ? NewId() : matchIdOrEmpty;

        if (!_sessions.TryGetValue(matchId, out var session))
        {
            session = new MatchSession(matchId);
            _sessions.Add(matchId, session);
        }

        // 找空槽位
        int slot = FindFreeSlot(session);
        if (slot < 0)
        {
            TargetError(sender, "Match is full.");
            return;
        }

        // 绑定到槽位
        var token = NewToken();
        session.Slots[slot].Token = token;
        session.Slots[slot].Conn = sender;
        session.Slots[slot].LastSeenUtc = DateTime.UtcNow;
        _connMap[sender.ClientId] = (matchId, slot);

        // 发 snapshot（最小：只告诉你 matchId/slot/服务器事件index）
        var snap = new Snapshot
        {
            matchId = matchId,
            slot = slot,
            serverLastEventIndex = session.ServerLastEventIndex
        };
        var snapJson = JsonUtility.ToJson(snap);

        // 只发给本人：你需要保存 matchId/token/lastEventIndex
        TargetJoined(sender, matchId, slot, token, snapJson);

        // 凑齐两人就开始（演示：写入一条 start 事件并广播）
        if (!session.Started && IsReady(session))
        {
            session.Started = true;
            var ev = session.AddEvent("StartGame", new StartGameEvent { PlayerId = 1 });
            BroadcastToSession(session, ev);
        }
    }

    // =======================
    // Client -> Server: Join or Create
    // =======================
    [ServerRpc(RequireOwnership = false)]
    public void SendServerRpc(NetworkConnection sender = null)
    {
        if (sender == null) return;

        if (!_connMap.TryGetValue(sender.ClientId, out var info))
        {
            TargetError(sender, "Not in a match.");
            return;
        }

        if (!_sessions.TryGetValue(info.matchId, out var session))
        {
            TargetError(sender, "Match missing.");
            return;
        }

        session.Slots[info.slot].LastSeenUtc = DateTime.UtcNow;

        // 写入事件日志 + 广播给本局两人
        var ev = session.AddEvent("DrawCard", new DrawCardEvent { PlayerId = 1});
        BroadcastToSession(session, ev);
    }

    // =======================
    // Client -> Server: Reconnect
    // lastEventIndex = 客户端已处理到的最后事件 index（没有则 -1）
    // =======================
    [ServerRpc(RequireOwnership = false)]
    public void ReconnectServerRpc(string matchId, string token, int lastEventIndex, NetworkConnection sender = null)
    {
        if (sender == null) return;

        if (!_sessions.TryGetValue(matchId, out var session))
        {
            TargetError(sender, "Match not found.");
            return;
        }

        int slot = FindSlotByToken(session, token);
        if (slot < 0)
        {
            TargetError(sender, "Bad token.");
            return;
        }

        // 如果 sender 之前有旧映射，先拆掉
        if (_connMap.TryGetValue(sender.ClientId, out var old))
        {
            DetachConnection(sender.ClientId, old.matchId, old.slot);
        }

        // 绑定新连接到原槽位
        session.Slots[slot].Conn = sender;
        session.Slots[slot].LastSeenUtc = DateTime.UtcNow;
        _connMap[sender.ClientId] = (matchId, slot);

        // 先发 snapshot（你可以在此恢复 UI / state）
        var snap = new Snapshot
        {
            matchId = matchId,
            slot = slot,
            serverLastEventIndex = session.ServerLastEventIndex
        };
        TargetSnapshot(sender, JsonUtility.ToJson(snap));

        // 再补发缺失事件：Index > lastEventIndex
        for (int i = 0; i < session.EventLog.Count; i++)
        {
            var ev = session.EventLog[i];
            if (ev.Index > lastEventIndex)
                TargetEvent(sender, ev);
        }

        TargetInfo(sender, $"Reconnected OK. Sent events > {lastEventIndex}.");
    }

    // =======================
    // Client -> Server: Send message within match
    // =======================
    [ServerRpc(RequireOwnership = false)]
    public void SendChatServerRpc(string message, NetworkConnection sender = null)
    {
        if (sender == null) return;

        if (!_connMap.TryGetValue(sender.ClientId, out var info))
        {
            TargetError(sender, "Not in a match.");
            return;
        }

        if (!_sessions.TryGetValue(info.matchId, out var session))
        {
            TargetError(sender, "Match missing.");
            return;
        }

        session.Slots[info.slot].LastSeenUtc = DateTime.UtcNow;

        // 写入事件日志 + 广播给本局两人
        var ev = session.AddEvent("Chat", new ChatEvent { PlayerId = 1, text = message });
        BroadcastToSession(session, ev);
    }

    // =======================
    // Server: broadcast event to both players in a session
    // =======================
    private void BroadcastToSession(MatchSession session, NetEvent ev)
    {
        for (int i = 0; i < 2; i++)
        {
            var conn = session.Slots[i].Conn;
            if (conn != null)
                TargetEvent(conn, ev);
        }
    }

    // =======================
    // Server: detach a connection mapping (best-effort)
    // =======================
    private void DetachConnection(int clientId, string matchId, int slot)
    {
        _connMap.Remove(clientId);

        if (_sessions.TryGetValue(matchId, out var session))
        {
            // 只清掉当前槽位的 Conn（token/状态保留，以便重连）
            if (session.Slots[slot].Conn != null && session.Slots[slot].Conn.ClientId == clientId)
            {
                session.Slots[slot].Conn = null;
                session.Slots[slot].LastSeenUtc = DateTime.UtcNow;

                // 记录一条“离线事件”（可选）
                var ev = session.AddEvent("StartGame", new StartGameEvent { PlayerId = 1 });
                BroadcastToSession(session, ev);
            }
        }
    }

    // =======================
    // Dedicated multi-match GC
    // =======================
    private void Update()
    {
        if (!IsServer) return;

        _gcTimer += Time.deltaTime;
        if (_gcTimer < 5f) return;
        _gcTimer = 0f;

        var now = DateTime.UtcNow;
        var remove = new List<string>();

        foreach (var kv in _sessions)
        {
            var s = kv.Value;

            bool bothOffline = (s.Slots[0].Conn == null && s.Slots[1].Conn == null);
            if (!bothOffline) continue;

            bool expired =
                (now - s.Slots[0].LastSeenUtc > KeepAlive) &&
                (now - s.Slots[1].LastSeenUtc > KeepAlive);

            if (expired) remove.Add(kv.Key);
        }

        foreach (var id in remove)
        {
            _sessions.Remove(id);
            // Debug.Log($"[Server] GC session {id}");
        }
    }

    public INetEventPayload DecodePayload(NetEvent ev)
    {
        var type = NetEventRegistry.GetPayloadType(ev.Type);
        if (type == null)
            return null;

        return (INetEventPayload)UnityEngine.JsonUtility
            .FromJson(ev.Payload, type);
    }

    // ===== Target RPCs (Server -> Client) =====
    [TargetRpc]
    private void TargetJoined(NetworkConnection conn, string matchId, int slot, string token, string snapshotJson)
    {
        Debug.Log($"[Client] Joined match={matchId}, slot={slot}, token={token}");
        Debug.Log($"[Client] Snapshot: {snapshotJson}");
        OnClientJoined?.Invoke(matchId, slot, token, snapshotJson);
    }

    [TargetRpc]
    private void TargetSnapshot(NetworkConnection conn, string snapshotJson)
    {
        Debug.Log($"[Client] Snapshot: {snapshotJson}");
    }

    [TargetRpc]
    private void TargetEvent(NetworkConnection conn, NetEvent ev)
    {
        var payload = DecodePayload(ev);
        string context = "";
        if (payload is ChatEvent chat)
            context = chat.text;
        if (payload is DrawCardEvent draw)
        {
            Debug.Log($"draw {UnityEngine.Random.Range(0, 15)}");
            context = "draw";
        }
        Debug.Log($"[Client] Event#{ev.Index} type={ev.Type} payload={context}");
        OnClientEvent?.Invoke(ev);
    }

    [TargetRpc]
    private void TargetInfo(NetworkConnection conn, string msg)
    {
        Debug.Log($"[Client] Info: {msg}");
    }

    [TargetRpc]
    private void TargetError(NetworkConnection conn, string err)
    {
        Debug.LogWarning($"[Client] Error: {err}");
    }

    // ===== Helpers =====
    private static string NewId() => Guid.NewGuid().ToString("N");
    private static string NewToken() => Guid.NewGuid().ToString("N");

    private static int FindFreeSlot(MatchSession s)
        => s.Slots[0].Conn == null ? 0 : (s.Slots[1].Conn == null ? 1 : -1);

    private static bool IsReady(MatchSession s)
        => s.Slots[0].Conn != null && s.Slots[1].Conn != null;

    private static int FindSlotByToken(MatchSession s, string token)
    {
        if (s.Slots[0].Token == token) return 0;
        if (s.Slots[1].Token == token) return 1;
        return -1;
    }
}