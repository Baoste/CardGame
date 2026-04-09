using Cinemachine;
using DG.Tweening;
using FishNet.Demo.AdditiveScenes;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MeshDestroy;

public class BoardView : MonoBehaviour, IViewClear
{
    [Header("Board Plane")]
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float boardWidth = 8f;
    [SerializeField] private float boardHeight = 4f;

    [Header("Area Split")]
    [SerializeField, Range(0.1f, 0.9f)] private float selfAreaHeightRatio = 0.5f;
    // 剩余部分自动给 opponent

    [Header("Card Deck")]
    [SerializeField] private PointCardDeck cardDeck;
    [SerializeField] private float dropDistance;
    [SerializeField] private float rotationAmount;

    [Header("Card Rotation")]
    [SerializeField] private Vector3 selfEuler = new Vector3(-90f, 0f, 0f);
    [SerializeField] private Vector3 opponentEuler = new Vector3(90f, 0f, 0f);

    [Header("Spawn Position")]
    [SerializeField] private Vector3 selfSpawnPosition;
    [SerializeField] private Vector3 opponentSpawnPosition;
    
    [Header("Hole Position")]
    [SerializeField] private Vector3 selfHoleTargetPosition;
    [SerializeField] private Vector3 opponentHoleTargetPosition;
    [SerializeField] private float fallDistance;

    private readonly List<GameObject> selfCards = new();
    private readonly List<GameObject> opponentCards = new();

    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ClearView()
    {
        selfCards.Clear();
        opponentCards.Clear();
    }

    public IEnumerator AddCard(GameObject instance, int playerId, CardVisualState cardState)
    {
        bool isOpponent = playerId != ClientGameState.playerSlot;
        
        Quaternion targetRotation = Quaternion.Euler(cardState == CardVisualState.Hole && isOpponent ? opponentEuler : selfEuler);
        instance.transform.rotation = targetRotation;
        PointCardInstance pointIns = instance.GetComponent<PointCardInstance>();

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

        pointIns.InitCardState(cardState, isOpponent);

        // 发牌
        if (cardState == CardVisualState.Hole)
        {
            yield return HoleCardReady(isOpponent, instance);
        }
        else
        {
            yield return CardReady(isOpponent, instance);
        }
    }

    public IEnumerator HoleCardReady(bool isOpponent, GameObject instance)
    {
        // 弹出动画
        float rotation = isOpponent ? 180f : 0f;
        StartCoroutine(cardDeck.EjectDisk(instance.transform, rotation));
        yield return new WaitForSeconds(0.5f);

        Vector3 targetPosition = isOpponent ? opponentHoleTargetPosition : selfHoleTargetPosition;
        Vector3 fallPosition = targetPosition + Vector3.down * fallDistance;

        Sequence seq = DOTween.Sequence();
        seq.Append(instance.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutBack));
        seq.Append(instance.transform.DOMove(fallPosition, 0.1f).SetEase(Ease.InCubic));
        yield return seq.WaitForCompletion();
        instance.GetComponent<PointCardShake>().CardShake();
    }


    public IEnumerator CardReady(bool isOpponent, GameObject instance)
    {
        yield return UpdateCardPositionsNormal(isOpponent, true);

        float minZ, maxZ, centerZ;
        float startX, moveDir;

        int countOnBoard = isOpponent ? opponentCards.Count - 1 : selfCards.Count - 1;
        float spacingFactor = countOnBoard < 4 ? 2f : 1;
        float cardSpacing = boardWidth / (countOnBoard + spacingFactor);
        float totalWidth = (countOnBoard - 1) * cardSpacing;
        float selfAreaHeight = boardHeight * selfAreaHeightRatio;
        float opponentAreaHeight = boardHeight - selfAreaHeightRatio * boardHeight;

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

        float fallDistance = 0.08f;
        Vector3 targetPosition = new Vector3(
            startX + (countOnBoard - 1) * moveDir * cardSpacing,
            boardCenter.y + fallDistance,
            centerZ
        );
        Vector3 fallPosition = targetPosition + Vector3.down * fallDistance;

        // 弹出动画
        StartCoroutine(cardDeck.EjectDisk(instance.transform, targetPosition));
        yield return new WaitForSeconds(0.5f);

        Sequence seq = DOTween.Sequence();
        seq.Append(instance.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutBack));
        seq.Append(instance.transform.DOMove(fallPosition, 0.1f).SetEase(Ease.InCubic));
        yield return seq.WaitForCompletion();

        instance.GetComponent<PointCardShake>().CardShake();
        Quaternion targetRotation = instance.transform.rotation * Quaternion.Euler(0, 0.5f, 0);
        instance.transform.DORotateQuaternion(targetRotation, 0.2f);
    }

    public IEnumerator RemoveCard(GameObject instance)
    {
        if (opponentCards.Remove(instance))
        {
            yield return DestroyCard(instance);
            yield return new WaitForSeconds(0.2f);
            yield return UpdateCardPositionsNormal(true, false);
        }
        if (selfCards.Remove(instance))
        {
            yield return DestroyCard(instance);
            yield return new WaitForSeconds(0.2f);
            yield return UpdateCardPositionsNormal(false, false);
        }
    }

    public IEnumerator RemoveCardInstant(GameObject instance)
    {
        if (opponentCards.Remove(instance))
        {
            Destroy(instance);
            yield return UpdateCardPositionsNormal(true, false);
        }
        if (selfCards.Remove(instance))
        {
            Destroy(instance);
            yield return UpdateCardPositionsNormal(false, false);
        }
    }

    public IEnumerator RemoveOneSideCards(int loserId)
    {
        bool isOpponent = loserId != ClientGameState.playerSlot;
        if (isOpponent)
        {
            for (int i = opponentCards.Count - 1; i >= 0; i--)
            {
                GameObject obj = opponentCards[i];
                opponentCards.RemoveAt(i);
                StartCoroutine(DestroyCard(obj));
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
        else
        {
            for (int i = selfCards.Count - 1; i >= 0; i--)
            {
                GameObject obj = selfCards[i];
                selfCards.RemoveAt(i);
                StartCoroutine(DestroyCard(obj));
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }

    public IEnumerator RemoveAllCards()
    {
        for (int i = opponentCards.Count - 1; i >= 0; i--)
        {
            GameObject obj = opponentCards[i];
            opponentCards.RemoveAt(i);
            StartCoroutine(DestroyCard(obj));
            yield return new WaitForSecondsRealtime(0.1f);
        }
        for (int i = selfCards.Count - 1; i >= 0; i--)
        {
            GameObject obj = selfCards[i];
            selfCards.RemoveAt(i);
            StartCoroutine(DestroyCard(obj));
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    public IEnumerator UpdateCardPositionsNormal(bool isOpponent, bool isAdd)
    {
        if (isOpponent)
            yield return LayoutOneSideNormal(opponentCards, true, isAdd);
        else
            yield return LayoutOneSideNormal(selfCards, false, isAdd);
    }

    private IEnumerator LayoutOneSideNormal(List<GameObject> cards, bool isOpponent, bool isAdd)
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

        int endCount = isAdd ? cards.Count - 2 : cards.Count - 1;
        for (int i = endCount; i >= 1; i--)
        {
            GameObject card = cards[i];
            if (card == null) continue;

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

    //public IEnumerator UpdateCardPositionsBounce(bool isOpponent)
    //{
    //    if (isOpponent)
    //        yield return LayoutOneSideBounce(opponentCards, true);
    //    else
    //        yield return LayoutOneSideBounce(selfCards, false);
    //}

    //private IEnumerator LayoutOneSideBounce(List<GameObject> cards, bool isOpponent)
    //{
    //    int countOnBoard = cards.Count - 1;
    //    if (countOnBoard == 0) yield break;

    //    float selfAreaHeight = boardHeight * selfAreaHeightRatio;
    //    float opponentAreaHeight = boardHeight - selfAreaHeightRatio * boardHeight;

    //    // 假设这个平面在 XZ 平面上：
    //    // X 控制左右，Z 控制上下（近/远）
    //    float minZ, maxZ, centerZ;
    //    float startX, moveDir;

    //    float spacingFactor = countOnBoard < 4 ? 2.3f : 1;
    //    float cardSpacing = boardWidth / (countOnBoard + spacingFactor);
    //    float totalWidth = (countOnBoard - 1) * cardSpacing;

    //    if (isOpponent)
    //    {
    //        // 上半区
    //        minZ = boardCenter.z;
    //        maxZ = boardCenter.z + opponentAreaHeight;
    //        centerZ = (minZ + maxZ) * 0.5f;
    //        startX = boardCenter.x + totalWidth * 0.5f;
    //        moveDir = -1;
    //    }
    //    else
    //    {
    //        // 下半区
    //        minZ = boardCenter.z - selfAreaHeight;
    //        maxZ = boardCenter.z;
    //        centerZ = (minZ + maxZ) * 0.5f;
    //        startX = boardCenter.x - totalWidth * 0.5f;
    //        moveDir = 1;
    //    }

    //    float shootDuring = 0.6f;

    //    // 只有一张
    //    if (countOnBoard == 1)
    //    {
    //        GameObject card = cards[1];
    //        Vector3 targetPos = new Vector3(
    //            startX,
    //            boardCenter.y,
    //            centerZ
    //        );
    //        card.transform.DOMove(targetPos, shootDuring).SetEase(Ease.OutQuad);
    //        yield break;
    //    }

    //    // 最后一张直接射出
    //    Vector3 fakeTarget = new Vector3(
    //        boardCenter.x - moveDir * boardWidth * 0.5f,
    //        boardCenter.y,
    //        centerZ
    //    );
    //    cards[cards.Count - 1].transform.DOMove(fakeTarget, shootDuring);

    //    for (int i = cards.Count - 1; i >= 1; i--)
    //    {
    //        GameObject card = cards[i];
    //        if (card == null) continue;

    //        // 有碰撞后，其他到指定位置
    //        PointCardInstance ins = card.GetComponent<PointCardInstance>();
    //        yield return new WaitUntil(() => ins.touchAnotherCard);
    //        Vector3 targetPos = new Vector3(
    //            startX + (i - 1) * moveDir * cardSpacing,
    //            boardCenter.y,
    //            centerZ
    //        );
    //        card.transform.DOKill();
    //        float during = Mathf.Abs(card.transform.position.x - targetPos.x);
    //        during = Mathf.Max(0.25f, during * 1.7f);
    //        card.transform.DOMove(targetPos, during).SetEase(Ease.OutCubic);
    //    }
    //}

    private IEnumerator DestroyCard(GameObject instance)
    {
        MeshDestroy mesh = instance.GetComponentInChildren<MeshDestroy>();
        GameObject tmp = mesh.transform.parent.gameObject;
        mesh.transform.parent.parent = transform.parent.parent;

        PointCardInstance point = instance.GetComponentInChildren<PointCardInstance>();
        int cutCascades = point.cardVisualState == CardVisualState.Hole ? 4 : 1 + point.point / 3;
        List<PartMesh> submeshes = mesh.DestroyMesh(cutCascades);
        Destroy(tmp);
        Destroy(instance);

        Time.timeScale = 0.01f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        yield return new WaitForSeconds(1f);
        Sequence seq = DOTween.Sequence();
        seq.OnComplete(() =>
        {
            foreach (PartMesh part in submeshes)
                part.GameObject.GetComponent<DissolutionController>().DestroySelf();
        });
        yield return seq.WaitForCompletion();
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
        Gizmos.DrawLine(selfHoleTargetPosition, selfHoleTargetPosition + Vector3.down * fallDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(opponentSpawnPosition, 0.05f);
        Gizmos.DrawSphere(opponentHoleTargetPosition, 0.05f);
    }

}