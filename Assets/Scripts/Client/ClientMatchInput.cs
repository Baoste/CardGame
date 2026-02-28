using Game.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientMatchInput : MonoBehaviour
{
    public MatchGateway gateway;
    public TMP_InputField inputField;
    public TMP_InputField inputField1;

    [Header("Persisted (copy from log for quick test)")]
    public string matchId;
    public string token;

    [Header("Client state")]
    public int lastEventIndex = -1;

    void OnEnable()
    {
        MatchGateway.OnClientJoined += OnJoined;
        MatchGateway.OnClientEvent += OnEvent;
    }

    void OnDisable()
    {
        MatchGateway.OnClientJoined -= OnJoined;
        MatchGateway.OnClientEvent -= OnEvent;
    }

    void OnJoined(string matchId, int slot, string token, string snapshotJson)
    {
        inputField.text = matchId;

        Debug.Log($"[UI] Joined match {matchId}, slot {slot}");
    }

    void OnEvent(Game.Domain.NetEvent ev)
    {
        if (ev.Index > lastEventIndex)
            lastEventIndex = ev.Index;
    }


    void Update()
    {
        if (gateway == null) return;

        // C：创建新局
        if (Input.GetKeyDown(KeyCode.F1))
        {
            gateway.JoinOrCreateServerRpc("");
            inputField.text = "";
            Debug.Log("[Client] Requested create match");
        }

        // J：加入指定 matchId
        if (Input.GetKeyDown(KeyCode.F2))
        {
            string matchId = inputField.text.Trim();
            gateway.JoinOrCreateServerRpc(matchId);
            Debug.Log($"[Client] Requested join matchId={matchId}");
        }

        // M：发消息（写入本局 eventlog 并广播给同局两人）
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ChatCommand cmd = new ChatCommand { PlayerId = 1, chatContext = inputField1.text };
            gateway.SendCommandServerRpc("Chat", JsonUtility.ToJson(cmd));
        }

        // R：重连（需要你先断线再连上服务器，然后按 R）
        if (Input.GetKeyDown(KeyCode.F4))
        {
            gateway.ReconnectServerRpc(matchId, token, lastEventIndex);
            Debug.Log($"[Client] Requested reconnect match={matchId} lastEventIndex={lastEventIndex}");
        }

        // 发牌
        if (Input.GetKeyDown(KeyCode.F5))
        {
            DrawCardCommand cmd = new DrawCardCommand { PlayerId = 1 };
            gateway.SendCommandServerRpc("DrawCard", JsonUtility.ToJson(cmd));
        }
    }

    // 你在真实项目里会这样更新 lastEventIndex：
    // - 把 TargetEvent(...) 里打印的 ev.Index 存到这里（例如通过事件转发或单例）
    //
    // 这个最小示例为了“只给你完整可编译代码”，就不做跨脚本回调了。
    // 你要的话我可以把 TargetEvent 改为触发 C# event，然后这里订阅并更新 lastEventIndex。
}