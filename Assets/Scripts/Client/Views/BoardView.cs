using Cinemachine;
using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
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
    
    [Header("Hole Position")]
    [SerializeField] private Vector3 selfHoleTargetPosition;
    [SerializeField] private Vector3 opponentHoleTargetPosition;
    [SerializeField] private float spawnDistance;
    [SerializeField] private float fallDistance;

    private readonly List<GameObject> selfCards = new();
    private readonly List<GameObject> opponentCards = new();

    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public IEnumerator AddCard(GameObject instance, int playerId, bool isHoleCard)
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

        if (isHoleCard && isOpponent)
        {
            instance.GetComponent<PointCardInstance>().pointText.text = "";
            targetRotation = instance.transform.rotation * Quaternion.Euler(180, 0, 0);
            instance.transform.rotation = targetRotation;
        }

        if (isHoleCard)
            yield return HoleCardReady(isOpponent, instance);
        else
            yield return CardReady(isOpponent, instance);
    }

    public IEnumerator HoleCardReady(bool isOpponent, GameObject instance)
    {
        Vector3 targetPosition = isOpponent ? opponentHoleTargetPosition : selfHoleTargetPosition;
        float moveDir = isOpponent ? -1f : 1f;
        instance.transform.position = targetPosition + moveDir * Vector3.forward * spawnDistance;
        Vector3 fallPosition = targetPosition + Vector3.down * fallDistance;

        Sequence seq = DOTween.Sequence();
        seq.Append(instance.transform.DOMove(targetPosition, 0.3f).SetEase(Ease.OutBack));
        seq.Append(instance.transform.DOMove(fallPosition, 0.1f).SetEase(Ease.InCubic));
        yield return seq.WaitForCompletion();
        instance.GetComponent<PointCardShake>().CardShake();
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

        Quaternion targetRotation = instance.transform.rotation * Quaternion.Euler(0, 0.5f, 0);
        instance.transform.DORotateQuaternion(targetRotation, 0.2f);
        yield return UpdateCardPositionsBounce(isOpponent);
    }

    public IEnumerator RemoveCard(GameObject instance)
    {
        if (opponentCards.Remove(instance))
        {
            Destroy(instance);
            yield return UpdateCardPositionsNormal(true);
        }
        if (selfCards.Remove(instance))
        {
            Destroy(instance);
            yield return UpdateCardPositionsNormal(false);
        }
    }

    public IEnumerator UpdateCardPositionsNormal(bool isOpponent)
    {
        // TODO: 改成非弹射的更新方式
        if (isOpponent)
            yield return LayoutOneSide(opponentCards, true);
        else
            yield return LayoutOneSide(selfCards, false);
    }

    public IEnumerator UpdateCardPositionsBounce(bool isOpponent)
    {
        if (isOpponent)
            yield return LayoutOneSide(opponentCards, true);
        else
            yield return LayoutOneSide(selfCards, false);
    }

    private IEnumerator LayoutOneSide(List<GameObject> cards, bool isOpponent)
    {
        int countOnBoard = cards.Count - 1;
        if (countOnBoard == 0) yield break;

        float selfAreaHeight = boardHeight * selfAreaHeightRatio;
        float opponentAreaHeight = boardHeight - selfAreaHeightRatio * boardHeight;

        // 假设这个平面在 XZ 平面上：
        // X 控制左右，Z 控制上下（近/远）
        float minZ, maxZ, centerZ;
        float startX, moveDir;

        float spacingFactor = countOnBoard < 4 ? 2.3f : 1;
        float cardSpacing = boardWidth / (countOnBoard + spacingFactor);
        float totalWidth = (countOnBoard - 1) * cardSpacing;

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

        float shootDuring = 0.6f;

        // 只有一张
        if (countOnBoard == 1)
        {
            GameObject card = cards[1];
            Vector3 targetPos = new Vector3(
                startX,
                boardCenter.y,
                centerZ
            );
            card.transform.DOMove(targetPos, shootDuring).SetEase(Ease.OutQuad);
            yield break;
        }

        // 最后一张直接射出
        Vector3 fakeTarget = new Vector3(
            boardCenter.x - moveDir * boardWidth * 0.5f,
            boardCenter.y,
            centerZ
        );
        cards[cards.Count - 1].transform.DOMove(fakeTarget, shootDuring);

        for (int i = cards.Count - 1; i >= 1; i--)
        {
            GameObject card = cards[i];
            if (card == null) continue;

            // 有碰撞后，其他到指定位置
            PointCardInstance ins = card.GetComponent<PointCardInstance>();
            yield return new WaitUntil(() => ins.touchAnotherCard);
            Vector3 targetPos = new Vector3(
                startX + (i - 1) * moveDir * cardSpacing,
                boardCenter.y,
                centerZ
            );
            card.transform.DOKill();
            float during = Mathf.Abs(card.transform.position.x - targetPos.x);
            during = Mathf.Max(0.25f, during * 1.7f);
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
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(selfSpawnPosition, 0.05f);
        Gizmos.DrawSphere(selfHoleTargetPosition, 0.05f);
        Gizmos.DrawLine(selfHoleTargetPosition, selfHoleTargetPosition + Vector3.forward * spawnDistance);
        Gizmos.DrawLine(selfHoleTargetPosition, selfHoleTargetPosition + Vector3.down * fallDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(opponentSpawnPosition, 0.05f);
        Gizmos.DrawSphere(opponentHoleTargetPosition, 0.05f);
    }

}