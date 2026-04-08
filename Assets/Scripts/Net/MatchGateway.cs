using System;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;
using Game.Domain;
using Game.Server;
using Newtonsoft.Json;


public class MatchGateway : NetworkBehaviour
{
    public static event Action<string, int, string, string> OnClientJoined;
    public static event Action<NetEvent> OnClientEvent;
    public static event Action<string> OnClientSnapshot;
   
    private CommandResult ProcessCommand(MatchSession session, NetCommand cmd)
        => CommandDispatcher.Process(session, cmd);
    private bool ProcessEvent(NetEvent ev)
        => EventDispatcher.Process(ev);

    // Dedicated 多局：matchId -> session
    private readonly Dictionary<string, MatchSession> _sessions = new();

    // 当前连接在哪一局：conn.ClientId -> (matchId, slot)
    private readonly Dictionary<int, (string matchId, int slot)> _connMap = new();

    // 两人都离线后保留多久再回收
    private static readonly TimeSpan KeepAlive = TimeSpan.FromSeconds(5);
    private float _gcTimer;


    // =======================
    // Client -> Server: Send Command
    // =======================
    [ServerRpc(RequireOwnership = false)]
    public void SendCommandServerRpc(string type, string jsonData, int playerId = -1, NetworkConnection sender = null)
    {
        if (sender == null) return;

        if (!CommandDispatcher.map.ContainsKey(type))
        {
            TargetError(sender, $"Unknown command type: {type}");
            return;
        }

        if (type == "GetCtx")           ClientEffectContext.GetServerCtxDone = false;
        if (type == "GetGameState")     ClientGameState.GetServerGameStateDone = false;

        if (type == "JoinOrCreateMatch")
        {
            JoinOrCreateMatchCommand payload = JsonConvert.DeserializeObject<JoinOrCreateMatchCommand>(jsonData);

            // 如果这个连接已经在某局里，先踢掉旧映射（最小实现：直接覆盖）
            if (_connMap.TryGetValue(sender.ClientId, out var old))
            {
                DetachConnection(sender.ClientId, old.matchId, old.slot);
            }

            var matchId = string.IsNullOrWhiteSpace(payload.matchIdOrEmpty) ? NewId() : payload.matchIdOrEmpty;

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
            var snapJson = JsonConvert.SerializeObject(snap);

            // 只发给本人：你需要保存 matchId/token/lastEventIndex
            TargetJoined(sender, matchId, slot, token, snapJson);

            // 凑齐两人就开始（演示：写入一条 start 事件并广播）
            if (!session.Started && IsReady(session))
            {
                session.Started = true;

                var cmd = session.AddCommand(type, jsonData);
                CommandResult results = ProcessCommand(session, cmd);
                while (results.events.Count > 0)
                {
                    var res = results.events.Dequeue();
                    var ev = session.AddEvent(res.type, res.jsonData);
                    BroadcastToSession(session, ev);
                }
            }
        }
        else if (type == "LeaveMatch")
        {
            if (!_connMap.TryGetValue(sender.ClientId, out var info))
            {
                TargetError(sender, "Not in a match.");
                return;
            }
            DetachConnection(sender.ClientId, info.matchId, info.slot);
            // TargetInfo(sender, "Left the match.");
        }
        else
        {
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

            // cmd传给服务器处理
            session.Slots[info.slot].LastSeenUtc = DateTime.UtcNow;
            var cmd = session.AddCommand(type, jsonData);
            CommandResult results = ProcessCommand(session, cmd);

            while (results.events.Count > 0)
            {
                var res = results.events.Dequeue();
                if (!EventDispatcher.map.ContainsKey(res.type))
                {
                    TargetError(sender, $"Unknown command type: {res.type}");
                    return;
                }

                // 返回event，广播给client
                var ev = session.AddEvent(res.type, res.jsonData);
                if (playerId != -1)
                {
                    var conn = session.Slots[playerId].Conn;
                    TargetEvent(conn, ev);
                }
                else if (res.sendId != -1)
                {
                    var conn = session.Slots[res.sendId].Conn;
                    TargetEvent(conn, ev);
                }
                else
                {
                    BroadcastToSession(session, ev);
                }
            }
        }
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
        TargetSnapshot(sender, JsonConvert.SerializeObject(snap));

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
                Debug.Log($"[Server] Detach connection {clientId} from match {matchId}, slot {slot}");
                session.Slots[slot].Conn = null;
                session.Slots[slot].LastSeenUtc = DateTime.UtcNow;

                // 记录一条“离线事件”（可选）
                //var ev = session.AddEvent("StartGame", new StartGameEvent { PlayerId = 1 });
                //BroadcastToSession(session, ev);
            }
        }
    }

    // =======================
    // Dedicated multi-match GC
    // =======================
    private void Update()
    {
        if (!IsServerInitialized) return;

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
            Debug.Log($"[Server] GC session {id}");
        }
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
        if (ProcessEvent(ev))
        {
            OnClientEvent?.Invoke(ev);
        }
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