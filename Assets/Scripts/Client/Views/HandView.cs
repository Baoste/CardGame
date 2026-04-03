using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class HandView : MonoBehaviour
{
    [SerializeField] private bool isOpponent = false;

    [Header("Resolve Zone Size")]
    [SerializeField] private float zoneWidth = 6f;
    [SerializeField] private float zoneHeight = 2.5f;
    [SerializeField] private Transform cameraTransform;

    [Header("Card Layout")]
    [SerializeField] private float cardSpacing = 1.8f;
    // [SerializeField] private float depthOffsetPerCard = 0.02f;

    [Header("Card Rotation")]
    [SerializeField] private Vector3 cardEuler = new Vector3(90f, 0f, 0f);
    [SerializeField] private SplineContainer splineContainer;

    [Header("Drag Settings")]
    [SerializeField] private BoxCollider dragValidArea; // 手牌安全区域，松手时如果不在这里就销毁

    [Header("Draw Anim Settings")]
    [SerializeField] private Transform skillCardsDeck;
    [SerializeField] private Vector3 instantiatePosition;
    [SerializeField] private float dropDistance;
    private Vector3 dropRotation;
    private Vector3 deckOriginalPosition;

    [HideInInspector] public List<GameObject> skillCardInstances = new();

    private void Awake()
    {
        if (!isOpponent)
            transform.up = -cameraTransform.forward;

        dropRotation = -skillCardsDeck.up;
        deckOriginalPosition = skillCardsDeck.transform.position;
    }

    public IEnumerator AddCard(GameObject instance)
    {
        skillCardInstances.Add(instance);
        BindDragComponent(instance);
        yield return DrawSkillCardAnimation(instance);
        // yield return new WaitForSeconds(0.5f);
        yield return UpdateCardPositions(0.5f);
    }

    public IEnumerator RemoveCard(GameObject instance)
    {
        if (skillCardInstances.Remove(instance))
        {
            yield return UpdateCardPositions(0.15f);
        }
    }

    private void BindDragComponent(GameObject instance)
    {
        if (isOpponent)
            return;

        SkillCardDraggable dragCard = instance.AddComponent<SkillCardDraggable>();
        SkillCardHover hoverCard = instance.AddComponent<SkillCardHover>();

        int cardId = instance.GetComponent<SkillCardInstance>().cardId;
        int instanceId = instance.GetComponent<SkillCardInstance>().instanceId;
        dragCard.Init(cardId, instanceId);
        hoverCard.Init();
    }

    private IEnumerator DrawSkillCardAnimation(GameObject instance)
    {
        // init
        skillCardsDeck.transform.position = deckOriginalPosition + dropRotation * dropDistance;
        instance.transform.position = instantiatePosition;
        instance.transform.rotation = isOpponent ? Quaternion.Euler(-47.6f, -175.862f, 86.442f) : Quaternion.Euler(-52, 0, 90);
        // yield return new WaitForSeconds(100f);

        float moveDir = isOpponent ? -1 : -1;

        Sequence seq = DOTween.Sequence();
        seq.Append(instance.transform.DOMove(instantiatePosition + moveDir * instance.transform.right * 0.05f, 0.15f));
        seq.Append(instance.transform.DOMove(instantiatePosition - moveDir * instance.transform.right * 1f, 0.5f).SetEase(Ease.OutBack));
        seq.Join(skillCardsDeck.transform.DOLocalMove(deckOriginalPosition, 0.3f).SetEase(Ease.OutBack));

        yield return seq.WaitForCompletion();
    }

     public IEnumerator UpdateCardPositions(float duration)
     {
         LayoutCards(duration);
         yield return new WaitForSeconds(duration);
     }

    private void LayoutCards(float duration)
    {
        if (skillCardInstances.Count == 0) return;

        float spacing = cardSpacing;
        if (skillCardInstances.Count > 1)
        {
            float maxAllowedSpacing = zoneWidth / (skillCardInstances.Count - 1);
            spacing = Mathf.Min(cardSpacing, maxAllowedSpacing);
        }

        float totalWidth = (skillCardInstances.Count - 1) * spacing;

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
            //if (isOpponent)
            //{
            //    targetRotation *= Quaternion.Euler(0f, 180f, 0f);
            //}

            card.transform.DOMove(targetPos, duration);
            card.transform.DORotateQuaternion(targetRotation, duration);
        }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        DrawRect(transform.position, transform.rotation, zoneWidth, zoneHeight);
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(instantiatePosition, 0.02f);
        Gizmos.DrawLine(instantiatePosition, instantiatePosition + -skillCardsDeck.up.normalized * dropDistance);
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
