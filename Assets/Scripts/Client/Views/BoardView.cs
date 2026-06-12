using Cinemachine;
using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static MeshDestroy;
using static Unity.Burst.Intrinsics.X86.Avx;

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

    [Header("Destroy")]
    [SerializeField] private GameObject decalPrefab;
    [SerializeField] private GameObject lazer;
    [SerializeField] private AnimationCurve lazerRotCurve;

    private readonly List<GameObject> selfCards = new();
    private readonly List<GameObject> opponentCards = new();

    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        // lazer.SetActive(false);
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

        instance.GetComponent<PointCardController>().SetIsOpponent(isOpponent);
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

    public IEnumerator MoveCard(GameObject instance, int fromPlayerId)
    {
        bool isFromOpponent = fromPlayerId != ClientGameState.playerSlot;

        if (isFromOpponent)
        {
            opponentCards.Remove(instance);
            yield return UpdateCardPositionsNormal(true, false);
            
            // move up
            Sequence seq = DOTween.Sequence();
            seq.Append(instance.transform.DOMoveY(instance.transform.position.y + 0.5f, 0.5f));
            seq.Append(instance.transform.DOMoveZ(instance.transform.position.z - 1.2f, 0.5f));
            yield return seq.WaitForCompletion();
            
            selfCards.Add(instance);
            instance.GetComponent<PointCardController>().SetIsOpponent(false);
            yield return UpdateCardPositionsNormal(false, false);
        }
        else
        {
            selfCards.Remove(instance);
            yield return UpdateCardPositionsNormal(false, false);

            // move up
            Sequence seq = DOTween.Sequence();
            seq.Append(instance.transform.DOMoveY(instance.transform.position.y + 0.5f, 0.5f));
            seq.Append(instance.transform.DOMoveZ(instance.transform.position.z + 1.2f, 0.5f));
            yield return seq.WaitForCompletion();

            opponentCards.Add(instance);
            instance.GetComponent<PointCardController>().SetIsOpponent(true);
            yield return UpdateCardPositionsNormal(true, false);
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

        // 移到目标位置
        AudioManager.Instance.Play("PointCardFly");

        PointCardController pcc = instance.GetComponent<PointCardController>();
        pcc.stateMachine.ChangeState(pcc.onBoardState);
        Sequence seq = DOTween.Sequence();
        seq.Append(instance.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutBack));
        seq.Append(instance.transform.DOMove(fallPosition, 0.1f).SetEase(Ease.InCubic));
        yield return seq.WaitForCompletion();

        AudioManager.Instance.Play("PointCardFall");
        instance.GetComponent<PointCardShake>().CardShake();
    }

    public IEnumerator HoleCardFlip()
    {
        Sequence seq = DOTween.Sequence();
        
        seq.Append(opponentCards[0].transform.DORotate(opponentEuler + new Vector3(180f, 0, 0), 0.5f).SetEase(Ease.OutBack));
        yield return seq.WaitForCompletion();
    }


    public IEnumerator CardReady(bool isOpponent, GameObject instance)
    {
        ClientEffectContext.isDrawingPointCard = true;
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

        // 移到目标位置
        AudioManager.Instance.Play("PointCardFly");

        PointCardController pcc = instance.GetComponent<PointCardController>();
        pcc.stateMachine.ChangeState(pcc.onBoardState);
        Sequence seq = DOTween.Sequence();
        seq.Append(instance.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutBack));
        seq.Append(instance.transform.DOMove(fallPosition, 0.1f).SetEase(Ease.InCubic));
        yield return seq.WaitForCompletion();

        AudioManager.Instance.Play("PointCardFall");

        instance.GetComponent<PointCardShake>().CardShake();
        Quaternion targetRotation = instance.transform.rotation * Quaternion.Euler(0, 0.5f, 0);
        instance.transform.DORotateQuaternion(targetRotation, 0.2f);

        ClientEffectContext.isDrawingPointCard = false;
    }

    public void GenerateLazer(Vector3 cardPosition, List<GameObject> objs)
    {
        foreach (GameObject obj in objs) 
        {
            selfCards.Remove(obj);
            opponentCards.Remove(obj);
        }
        StartCoroutine(_GenerateLazer(cardPosition));
    }

    private IEnumerator _GenerateLazer(Vector3 cardPosition)
    {
        Vector3 initPosition = lazer.transform.position;
        initPosition.z = cardPosition.z;
        lazer.transform.position = initPosition;
        lazer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
        lazer.SetActive(true);

        // TODO: 激光特效
        Sequence seq = DOTween.Sequence();
        seq.Append(
            lazer.transform
                .DORotate(new Vector3(0, 0, 89), 1.3f)
                .SetEase(lazerRotCurve)
        );
        yield return seq.WaitForCompletion();

        yield return new WaitForSecondsRealtime(1f);
        lazer.SetActive(false);
    }

    public IEnumerator RemoveCard(GameObject instance)
    {
        if (opponentCards.Remove(instance))
        {
            StartCoroutine(DestroyCard(instance));
            yield return new WaitForSecondsRealtime(0.2f);
            yield return UpdateCardPositionsNormal(true, false);
        }
        if (selfCards.Remove(instance))
        {
            StartCoroutine(DestroyCard(instance));
            yield return new WaitForSecondsRealtime(0.2f);
            yield return UpdateCardPositionsNormal(false, false);
        }
    }

    public IEnumerator RemoveHoleCard(int loserId)
    {
        bool isOpponent = loserId != ClientGameState.playerSlot;
        if (isOpponent)
        {
            GameObject obj = opponentCards[0];
            opponentCards.RemoveAt(0);
            yield return DestroyHoleCard(obj);
        }
        else
        {
            GameObject obj = selfCards[0];
            selfCards.RemoveAt(0);
            yield return DestroyHoleCard(obj);
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
                obj.AddComponent<DissolutionController>().DestroySelf();
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
        else
        {
            for (int i = selfCards.Count - 1; i >= 0; i--)
            {
                GameObject obj = selfCards[i];
                selfCards.RemoveAt(i);
                obj.AddComponent<DissolutionController>().DestroySelf();
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

            Quaternion targetRotation = card.transform.rotation * Quaternion.Euler(0, 0.5f, 0);
            card.transform.DORotateQuaternion(targetRotation, 0.2f);
        }
    }

    private IEnumerator DestroyCard(GameObject instance, float timeScaleDelay = 0.01f)
    {
        // up and down and shake
        Sequence upDownSeq = DOTween.Sequence();
        upDownSeq.Append(instance.transform.DOMoveY(instance.transform.position.y + 0.15f, 0.4f).SetEase(Ease.OutCubic));
        upDownSeq.Append(instance.transform.DOMoveY(instance.transform.position.y, 0.05f).SetEase(Ease.InCubic));
        yield return upDownSeq.WaitForCompletion();

        impulseSource.GenerateImpulse();
        AudioManager.Instance.Play("DiscardPointCard");

        PointCardController pc = instance.GetComponent<PointCardController>();
        pc.stateMachine.ChangeState(pc.discardState);

        // mesh destroy
        MeshDestroy mesh = instance.GetComponentInChildren<MeshDestroy>();
        GameObject tmp = mesh.transform.parent.gameObject;
        mesh.transform.parent.parent = transform.parent.parent;

        PointCardInstance point = instance.GetComponentInChildren<PointCardInstance>();
        int cutCascades = point.cardVisualState == CardVisualState.Hole ? 4 : 1 + point.point / 3;
        List<PartMesh> submeshes = mesh.DestroyMesh(cutCascades);
        Destroy(tmp);

        // decal
        DecalProjector decal = Instantiate(decalPrefab, instance.transform.position, Quaternion.Euler(90, 0, 0)).GetComponent<DecalProjector>();

        // mesh split
        Time.timeScale = 0.01f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSecondsRealtime(timeScaleDelay);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        yield return new WaitForSecondsRealtime(1f);
        Sequence seq = DOTween.Sequence();
        seq.OnComplete(() =>
        {
            foreach (PartMesh part in submeshes)
            {
                DissolutionController dc = part.GameObject.GetComponent<DissolutionController>();
                dc.DestroySelf();
            }
        });
        yield return seq.WaitForCompletion();

        // decal destroy
        yield return new WaitForSecondsRealtime(2f);
        DOTween.To(() => decal.fadeFactor, x => decal.fadeFactor = x, 0f, 1f);
        Destroy(decal.gameObject, 1.5f);
        Destroy(instance);
    }

    private IEnumerator DestroyHoleCard(GameObject instance)
    {
        Vector3 originalPos = instance.transform.position;

        // up and down and shake
        Sequence upDownSeq = DOTween.Sequence();
        upDownSeq.Append(instance.transform.DOMoveY(instance.transform.position.y + 0.25f, 0.8f).SetEase(Ease.OutCubic));
        upDownSeq.Append(instance.transform.DOMoveY(instance.transform.position.y + 0.2f, 0.05f).SetEase(Ease.InCubic));
        yield return upDownSeq.WaitForCompletion();
        //impulseSource.GenerateImpulse();
        AudioManager.Instance.Play("DiscardPointCard");

        PointCardController pc = instance.GetComponent<PointCardController>();
        pc.stateMachine.ChangeState(pc.discardState);

        // lighting generation
        SceneViewManager.viewAnimController.LightingManager.GenerateLighting(originalPos);
        yield return new WaitForSecondsRealtime(0.05f);

        // mesh destroy
        MeshDestroy mesh = instance.GetComponentInChildren<MeshDestroy>();
        GameObject tmp = mesh.transform.parent.gameObject;
        mesh.transform.parent.parent = transform.parent.parent;

        PointCardInstance point = instance.GetComponentInChildren<PointCardInstance>();
        int cutCascades = point.cardVisualState == CardVisualState.Hole ? 4 : 1 + point.point / 3;
        List<PartMesh> submeshes = mesh.DestroyMesh(3, forceMagnification: 3f);
        Destroy(tmp);

        // mesh split
        Time.timeScale = 0.01f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        yield return new WaitForSecondsRealtime(1f);
        Sequence seq = DOTween.Sequence();
        seq.OnComplete(() =>
        {
            foreach (PartMesh part in submeshes)
                part.GameObject.GetComponent<DissolutionController>().DestroySelf();
        });
        yield return seq.WaitForCompletion();

        // decal destroy
        Destroy(instance);
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