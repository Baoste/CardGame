using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using Game.Domain;
using FishNet.Demo.AdditiveScenes;

public class HandView : MonoBehaviour
{
    [Header("Resolve Zone Size")]
    [SerializeField] private float zoneWidth = 6f;
    [SerializeField] private float zoneHeight = 2.5f;

    [Header("Card Layout")]
    [SerializeField] private float cardSpacing = 1.8f;
    [SerializeField] private float depthOffsetPerCard = 0.02f;
    [SerializeField] private float animationDuration = 0.2f;

    [Header("Card Rotation")]
    [SerializeField] private Vector3 cardEuler = new Vector3(90f, 0f, 0f);


    [SerializeField] private SplineContainer splineContainer;

    [Header("Drag Settings")]
    [SerializeField] private BoxCollider dragValidArea; // 手牌安全区域，松手时如果不在这里就销毁
    [SerializeField] private float dragFollowDepth = 8f; // 鼠标拖拽时离摄像机多远

    private readonly List<GameObject> skillCardInstances = new();

    private void Start()
    {
        transform.up = -Camera.main.transform.forward;
    }

    public IEnumerator AddCard(GameObject instance, int playerId)
    {
        skillCardInstances.Add(instance);
        BindDragComponent(instance);
        yield return UpdateCardPositions(0.15f, playerId);
    }

    public IEnumerator RemoveCard(GameObject instance, int playerId)
    {
        if (skillCardInstances.Remove(instance))
        {
            Destroy(instance);
            yield return UpdateCardPositions(0.15f, playerId);
        }
    }

    private void BindDragComponent(GameObject instance)
    {
        SkillCardDraggable dragCard = instance.GetComponent<SkillCardDraggable>();
        if (dragCard == null)
            dragCard = instance.AddComponent<SkillCardDraggable>();

        int cardId = instance.GetComponent<SkillCardInstance>().cardId;
        int instanceId = instance.GetComponent<SkillCardInstance>().instanceId;
        dragCard.Init(this, cardId, instanceId);
    }

     public IEnumerator UpdateCardPositions(float duration, int playerId)
    {
        LayoutCards(duration, playerId);
        yield return new WaitForSeconds(duration);
    }

    private void LayoutCards(float duration, int playerId)
    {
        if (skillCardInstances.Count == 0) return;

        float spacing = cardSpacing;
        if (skillCardInstances.Count > 1)
        {
            float maxAllowedSpacing = zoneWidth / (skillCardInstances.Count - 1);
            spacing = Mathf.Min(cardSpacing, maxAllowedSpacing);
        }

        float totalWidth = (skillCardInstances.Count - 1) * spacing;
        bool isOpponent = playerId != ClientGameState.playerSlot;

        for (int i = 0; i < skillCardInstances.Count; i++)
        {
            GameObject card = skillCardInstances[i];
            if (card == null) continue;

            float localX = -totalWidth * 0.5f + i * spacing;
            float localZ = 0f; // 所有牌严格排成一条线，不做前后错位

            Vector3 localPos = new Vector3(localX, 0f, localZ);
            Vector3 targetPos = transform.TransformPoint(localPos);
            card.GetComponent<SkillCardInstance>().originalPos = targetPos;

            Quaternion targetRotation = transform.rotation * Quaternion.Euler(cardEuler);

            // 对手牌翻面
            if (isOpponent)
            {
                targetRotation *= Quaternion.Euler(0f, 180f, 0f);
            }

            card.transform.DOMove(targetPos, duration);
            card.transform.DORotateQuaternion(targetRotation, duration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        DrawRect(transform.position, transform.rotation, zoneWidth, zoneHeight);
    }

    private void DrawRect(Vector3 center, Quaternion rotation, float width, float height)
    {
        Vector3 a = center + rotation * new Vector3(-width * 0.5f, 0f, -height * 0.5f);
        Vector3 b = center + rotation * new Vector3(width * 0.5f, 0f, -height * 0.5f);
        Vector3 c = center + rotation * new Vector3(width * 0.5f, 0f, height * 0.5f);
        Vector3 d = center + rotation * new Vector3(-width * 0.5f, 0f, height * 0.5f);

        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }

    public bool IsOutsideValidArea(Vector3 worldPos)
    {
        if (dragValidArea == null)
            return false;

        Vector3 local = dragValidArea.transform.InverseTransformPoint(worldPos);
        Vector3 half = dragValidArea.size * 0.5f;

        return Mathf.Abs(local.x) > half.x ||
               Mathf.Abs(local.y) > half.y ||
               Mathf.Abs(local.z) > half.z;
    }

}
