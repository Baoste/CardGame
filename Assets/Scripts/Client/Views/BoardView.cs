using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [Header("Board Plane")]
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float boardWidth = 8f;
    [SerializeField] private float boardHeight = 4f;

    [Header("Area Split")]
    [SerializeField, Range(0.1f, 0.9f)] private float selfAreaHeightRatio = 0.5f;
    // 剩余部分自动给 opponent

    [Header("Card Layout")]
    [SerializeField] private float cardSpacing = 1.2f;
    [SerializeField] private float depthOffsetPerCard = 0.01f;
    [SerializeField] private float animationDuration = 0.15f;

    [Header("Card Rotation")]
    [SerializeField] private Vector3 selfEuler = new Vector3(90f, 0f, 0f);
    [SerializeField] private Vector3 opponentEuler = new Vector3(90f, 0f, 0f);

    private readonly List<GameObject> selfCards = new();
    private readonly List<GameObject> opponentCards = new();

    public IEnumerator AddCard(GameObject instance, int playerId)
    {
        bool isOpponent = playerId != ClientGameState.playerSlot;

        if (isOpponent)
            opponentCards.Add(instance);
        else
            selfCards.Add(instance);

        yield return UpdateCardPositions(animationDuration);
    }

    public IEnumerator RemoveCard(GameObject instance)
    {
        selfCards.Remove(instance);
        opponentCards.Remove(instance);

        yield return UpdateCardPositions(animationDuration);
    }

    public IEnumerator UpdateCardPositions(float duration)
    {
        LayoutOneSide(selfCards, false, duration);
        LayoutOneSide(opponentCards, true, duration);

        yield return new WaitForSeconds(duration);
    }

    private void LayoutOneSide(List<GameObject> cards, bool isOpponent, float duration)
    {
        if (cards.Count == 0) return;

        float selfAreaHeight = boardHeight * selfAreaHeightRatio;
        float opponentAreaHeight = boardHeight - selfAreaHeightRatio * boardHeight;

        // 假设这个平面在 XZ 平面上：
        // X 控制左右，Z 控制上下（近/远）
        float minZ, maxZ, centerZ;

        if (isOpponent)
        {
            // 上半区
            minZ = boardCenter.z;
            maxZ = boardCenter.z + opponentAreaHeight;
            centerZ = (minZ + maxZ) * 0.5f;
        }
        else
        {
            // 下半区
            minZ = boardCenter.z - selfAreaHeight;
            maxZ = boardCenter.z;
            centerZ = (minZ + maxZ) * 0.5f;
        }

        float totalWidth = (cards.Count - 1) * cardSpacing;
        float startX = boardCenter.x - totalWidth * 0.5f;

        Quaternion targetRotation = Quaternion.Euler(isOpponent ? opponentEuler : selfEuler);

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            if (card == null) continue;

            Vector3 targetPos = new Vector3(
                startX + i * cardSpacing,
                boardCenter.y,
                centerZ + i * depthOffsetPerCard
            );

            card.transform.DOMove(targetPos, duration);
            card.transform.DORotateQuaternion(targetRotation, duration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 画出整个牌桌区域，方便在 Scene 里调
        Gizmos.color = Color.yellow;
        DrawRect(boardCenter, boardWidth, boardHeight);

        float selfAreaHeight = boardHeight * selfAreaHeightRatio;
        float opponentAreaHeight = boardHeight - selfAreaHeight;

        Vector3 selfCenter = new Vector3(
            boardCenter.x,
            boardCenter.y,
            boardCenter.z - boardHeight * 0.5f + selfAreaHeight * 0.5f
        );

        Vector3 opponentCenter = new Vector3(
            boardCenter.x,
            boardCenter.y,
            boardCenter.z + boardHeight * 0.5f - opponentAreaHeight * 0.5f
        );

        Gizmos.color = Color.green;
        DrawRect(selfCenter, boardWidth, selfAreaHeight);

        Gizmos.color = Color.red;
        DrawRect(opponentCenter, boardWidth, opponentAreaHeight);
    }

    private void DrawRect(Vector3 center, float width, float height)
    {
        Vector3 a = new Vector3(center.x - width * 0.5f, center.y, center.z - height * 0.5f);
        Vector3 b = new Vector3(center.x + width * 0.5f, center.y, center.z - height * 0.5f);
        Vector3 c = new Vector3(center.x + width * 0.5f, center.y, center.z + height * 0.5f);
        Vector3 d = new Vector3(center.x - width * 0.5f, center.y, center.z + height * 0.5f);

        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
}