using DG.Tweening;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveZoneView : MonoBehaviour, IViewClear
{
    [Header("Resolve Zone Size")]
    [SerializeField] private float zoneWidth = 6f;
    [SerializeField] private float zoneHeight = 2.5f;

    [Header("Card Layout")]
    [SerializeField] private float cardSpacing = 1.8f;
    // [SerializeField] private float depthOffsetPerCard = 0.02f;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private GameObject confirmBtn = null;
    [SerializeField] private GameObject confirmText = null;

    [Header("Card Rotation")]
    [SerializeField] private Vector3 cardEuler = new Vector3(90f, 0f, 0f);

    private readonly List<GameObject> resolveCards = new();
    public IReadOnlyList<GameObject> ResolveCards => resolveCards;

    public void ClearView()
    {
        resolveCards.Clear();
    }

    public IEnumerator AddCard(GameObject instance, int playerId, bool isShown, CardVisualState cardState)
    {
        bool isOpponent = playerId != ClientGameState.playerSlot && !isShown;
        if (confirmBtn != null && confirmText != null && !isOpponent)
        {
            confirmBtn.SetActive(true);
            confirmText.SetActive(true);
        }

        PointCardResolve pointIns = instance.GetComponent<PointCardResolve>();
        pointIns.InitCardState(cardState);

        resolveCards.Add(instance);
        yield return UpdateCardPositions(animationDuration, playerId, isShown);
    }

    public void ClearPeek()
    {
        ClearCardsToResolveCommand cmd = new ClearCardsToResolveCommand { playerId = ClientGameState.playerSlot, isPeekZone = true };
        ClientGameState.gateway.SendCommandServerRpc("ClearCardsToResolve", JsonConvert.SerializeObject(cmd));
    }

    public IEnumerator ClearCards()
    {
        if (confirmBtn != null)
        {
            confirmBtn.SetActive(false);
        }
        if (confirmText != null)
        {
            confirmText.SetActive(false);
        }

        foreach (var card in resolveCards)
        {
            if (card != null)
            {
                Destroy(card);
            }
        }
        resolveCards.Clear();
        yield return null;
    }

    public IEnumerator UpdateCardPositions(float duration, int playerId, bool isShown)
    {
        LayoutCards(duration, playerId, isShown);
        yield return new WaitForSeconds(duration);
    }

    private void LayoutCards(float duration, int playerId, bool isShown)
    {
        if (resolveCards.Count == 0) return;

        float spacing = cardSpacing;
        if (resolveCards.Count > 1)
        {
            float maxAllowedSpacing = zoneWidth / (resolveCards.Count - 1);
            spacing = Mathf.Min(cardSpacing, maxAllowedSpacing);
        }

        float totalWidth = (resolveCards.Count - 1) * spacing;
        bool isOpponent = playerId != ClientGameState.playerSlot && !isShown;

        for (int i = 0; i < resolveCards.Count; i++)
        {
            GameObject card = resolveCards[i];
            if (card == null) continue;

            float localX = -totalWidth * 0.5f + i * spacing;
            float localZ = 0f; // 所有牌严格排成一条线，不做前后错位

            Vector3 localPos = new Vector3(localX, 0f, localZ);
            Vector3 targetPos = transform.TransformPoint(localPos);

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
}