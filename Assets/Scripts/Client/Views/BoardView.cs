using Cinemachine;
using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private float depthOffsetPerCard = 0.01f;

    [Header("Card Rotation")]
    [SerializeField] private Vector3 selfEuler = new Vector3(90f, 0f, 0f);
    [SerializeField] private Vector3 opponentEuler = new Vector3(90f, 0f, 0f);

    [Header("Spawn Position")]
    [SerializeField] private Vector3 selfSpawnPosition;
    [SerializeField] private Vector3 opponentSpawnPosition;

    private readonly List<GameObject> selfCards = new();
    private readonly List<GameObject> opponentCards = new();

    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public IEnumerator AddCard(GameObject instance, int playerId)
    {
        bool isOpponent = playerId != ClientGameState.playerSlot;
        
        Quaternion targetRotation = Quaternion.Euler(isOpponent ? opponentEuler : selfEuler);
        instance.transform.rotation = targetRotation;

        if (isOpponent)
        {
            instance.transform.position = opponentSpawnPosition;
            opponentCards.Add(instance);
        }
        else
        {
            instance.transform.position = selfSpawnPosition;
            selfCards.Add(instance);
        }

        yield return CardReady(isOpponent, instance);
    }

    public IEnumerator CardReady(bool isOpponent, GameObject instance)
    {
        float selfAreaHeight = boardHeight * selfAreaHeightRatio;
        float opponentAreaHeight = boardHeight - selfAreaHeightRatio * boardHeight;
        float centerZ = isOpponent ? 2 * boardCenter.z + opponentAreaHeight : 2 * boardCenter.z - selfAreaHeight;
        centerZ *= 0.5f;
        float moveDir = isOpponent ? -1f : 1f;
        Vector3 fakeTarget = new Vector3(
            boardCenter.x + boardWidth * 0.5f * moveDir,
            boardCenter.y,
            centerZ
        );
        instance.transform.DOMove(fakeTarget, 0.5f);
        yield return new WaitForSeconds(0.5f);

        yield return UpdateCardPositions(isOpponent);
    }

    public IEnumerator RemoveCard(GameObject instance)
    {
        if (opponentCards.Remove(instance))
        {
            Destroy(instance);
            yield return UpdateCardPositions(true);
        }
        if (selfCards.Remove(instance))
        {
            Destroy(instance);
            yield return UpdateCardPositions(false);
        }
    }

    public IEnumerator UpdateCardPositions(bool isOpponent)
    {
        if (isOpponent)
            yield return LayoutOneSide(opponentCards, true);
        else
            yield return LayoutOneSide(selfCards, false);
    }

    private IEnumerator LayoutOneSide(List<GameObject> cards, bool isOpponent)
    {
        if (cards.Count == 0) yield break;

        float selfAreaHeight = boardHeight * selfAreaHeightRatio;
        float opponentAreaHeight = boardHeight - selfAreaHeightRatio * boardHeight;

        // 假设这个平面在 XZ 平面上：
        // X 控制左右，Z 控制上下（近/远）
        float minZ, maxZ, centerZ;
        float startX, moveDir;

        float spacingFactor = cards.Count < 4 ? 3 : 1;
        float cardSpacing = boardWidth / (cards.Count + spacingFactor);
        float totalWidth = (cards.Count - 1) * cardSpacing;

        if (isOpponent)
        {
            // 上半区
            minZ = boardCenter.z;
            maxZ = boardCenter.z + opponentAreaHeight;
            centerZ = (minZ + maxZ) * 0.5f;
            startX = boardCenter.x + totalWidth * 0.5f;
            moveDir = -1;
        }
        else
        {
            // 下半区
            minZ = boardCenter.z - selfAreaHeight;
            maxZ = boardCenter.z;
            centerZ = (minZ + maxZ) * 0.5f;
            startX = boardCenter.x - totalWidth * 0.5f;
            moveDir = 1;
        }

        // 只有一张
        if (cards.Count == 1)
        {
            GameObject card = cards[0];
            Vector3 targetPos = new Vector3(
                startX,
                boardCenter.y,
                centerZ
            );
            card.transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutQuad);
            yield break;
        }

        // 最后一张直接射出
        Vector3 fakeTarget = new Vector3(
            boardCenter.x - moveDir * boardWidth * 0.5f,
            boardCenter.y,
            centerZ
        );
        cards[cards.Count - 1].transform.DOMove(fakeTarget, 0.3f);

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            GameObject card = cards[i];
            if (card == null) continue;

            // 有碰撞后，其他到指定位置
            PointCardInstance ins = card.GetComponent<PointCardInstance>();
            yield return new WaitUntil(() => ins.touchAnotherCard);
            Vector3 targetPos = new Vector3(
                startX + i * moveDir * cardSpacing,
                boardCenter.y,
                centerZ
            );
            card.transform.DOKill();
            float during = Mathf.Abs(card.transform.position.x - targetPos.x);
            during = Mathf.Max(0.25f, during * 2f);
            card.transform.DOMove(targetPos, during).SetEase(Ease.OutCubic);
        }
    }

    public IEnumerator ShakeCards()
    {
        impulseSource.GenerateImpulse();
        foreach (var card in selfCards)
        {
            card.GetComponent<PointCardShake>().CardShake();
        }
        foreach (var card in opponentCards)
        {
            card.GetComponent<PointCardShake>().CardShake();
        }
        yield break;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(selfSpawnPosition, 0.05f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(opponentSpawnPosition, 0.05f);
    }

}