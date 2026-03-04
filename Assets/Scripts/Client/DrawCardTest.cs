using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawCardTest : MonoBehaviour
{
    // 按 playerId 存储在屏幕上显示的牌（用于重排）
    private static readonly Dictionary<int, List<GameObject>> handMap = new();
    private static Dictionary<int, GameObject> instanceMap = new();

    // Start is called before the first frame update
    void Start()
    {
        ProcessDispatcher.Register("DrawCardTest", DrawCard);
        ProcessDispatcher.Register("DiscardCardTest", DiscardCard);
        ProcessDispatcher.Register("ModifyPointTest", ModifyPoint);
    }

    // parameters[0]: int instanceId
    // parameters[1]: int pointChange
    public void ModifyPoint(object[] parameters)
    {
        int instanceId = (int)parameters[0];
        int pointChange = (int)parameters[1];

        int point = int.Parse(instanceMap[instanceId].GetComponentInChildren<TextMeshPro>().text) + pointChange;
        instanceMap[instanceId].GetComponentInChildren<TextMeshPro>().text = point.ToString();
    }


    // parameters[0]: int instanceId
    public void DiscardCard(object[] parameters)
    {
        int instanceId = (int)parameters[0];
        handMap[0].Remove(instanceMap[instanceId]);
        handMap[1].Remove(instanceMap[instanceId]);
        Destroy(instanceMap[instanceId]);
        instanceMap.Remove(instanceId);
    }


    // parameters[0]: int cardId
    // parameters[1]: int instanceId
    // parameters[2]: int playerId
    // parameters[3]: bool isHoleCard
    public void DrawCard(object[] parameters)
    {
        int cardId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        int playerId = (int)parameters[2];
        bool isHoreCard = (bool)parameters[3];

        Card card = CardDatabase.Get(cardId);
        if (card == null)
        {
            Debug.Log($"Card with id {cardId} not found");
            return;
        }

        // 生成卡牌
        GameObject cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        if (cardPrefab == null)
        {
            Debug.LogError("Card prefab not found at Resources/Prefabs/Card");
            return;
        }

        // 先创建对象，位置会由 ArrangeHand 重排
        GameObject newCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
        instanceMap[instanceId] = newCard;
        newCard.GetComponent<CardView>().cardId = cardId;
        newCard.GetComponent<CardView>().instanceId = instanceId;

        var text = newCard.GetComponentInChildren<TextMeshPro>();
        if (text != null && (playerId == ClientGameState.playerSlot || !isHoreCard))
            text.text = card.point.ToString();
        else
            text.text = "";

        // 加入 handMap 并重排该玩家的手牌显示
        if (!handMap.TryGetValue(playerId, out var list))
        {
            list = new List<GameObject>();
            handMap[playerId] = list;
        }
        list.Add(newCard);

        ArrangeHand(playerId);
    }

    // 将指定 playerId 的所有牌在屏幕上排成一排（上半屏/下半屏）
    private void ArrangeHand(int playerId)
    {
        if (!handMap.TryGetValue(playerId, out var list) || list.Count == 0)
            return;

        if (Camera.main == null)
            return;

        int n = list.Count;

        // 视口（Viewport）Y：己方在上半屏，对方在下半屏
        // 约定：playerId == 0 表示己方（上半屏）；其他 playerId 为对方（下半屏）
        float viewportY = (playerId != ClientGameState.playerSlot) ? 0.75f : 0.25f;

        // 可用宽度（视口单位），以及每张牌的间距（视卡数量自适应）
        float availableWidth = 0.9f; // 留白两侧各 0.1
        float spacing = Mathf.Min(0.18f, availableWidth / Mathf.Max(1, n)); // 最大间距限制，避免过分分散
        float startX = 0.5f - spacing * (n - 1) / 2f; // 居中起始 X

        // 将视口坐标转换为世界坐标所需的距离（camera 到 z=0 平面）
        float distance = Mathf.Abs(Camera.main.transform.position.z);

        for (int i = 0; i < n; i++)
        {
            float vx = startX + i * spacing;
            Vector3 worldPos = Camera.main.ViewportToWorldPoint(new Vector3(vx, viewportY, distance));
            // 固定在 z = 0 平面（或按需要调整）
            float z = 10f;
            list[i].transform.position = new Vector3(worldPos.x, worldPos.y, z);
            list[i].transform.rotation = Quaternion.identity;
        }
    }
}
