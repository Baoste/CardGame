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

    [SerializeField] private HandView handView;
    [SerializeField] private BoardView boardView;

    // Start is called before the first frame update
    void Start()
    {
        ProcessDispatcher.Register("DrawCardTest", DrawCard);
        ProcessDispatcher.Register("DrawSkillCardTest", DrawSkillCard);
        ProcessDispatcher.Register("DiscardCardTest", DiscardCard);
        ProcessDispatcher.Register("ModifyPointTest", ModifyPoint);
        handMap[0] = new List<GameObject>();
        handMap[1] = new List<GameObject>();
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

        bool isOpponent = playerId != ClientGameState.playerSlot;

        GameObject instace = CardViewCreator.Instance.CreateCardInstace(cardId, instanceId, transform.position, Quaternion.identity);
        if (isHoreCard && isOpponent)
            instace.GetComponent<PointCardInstance>().pointText.text = "";

        handMap[playerId].Add(instace);
        instanceMap[instanceId] = instace;
        StartCoroutine(boardView.AddCard(instace, playerId));
    }

    // parameters[0]: int cardId
    // parameters[1]: int instanceId
    // parameters[2]: int playerId
    public void DrawSkillCard(object[] parameters)
    {
        int cardId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        int playerId = (int)parameters[2];

        bool isOpponent = playerId != ClientGameState.playerSlot;

        GameObject instace = CardViewCreator.Instance.CreateCardInstace(cardId, instanceId, transform.position, Quaternion.identity);

        handMap[playerId].Add(instace);
        instanceMap[instanceId] = instace;

        if (isOpponent) return;
        StartCoroutine(handView.AddCard(instace));
    }
}
