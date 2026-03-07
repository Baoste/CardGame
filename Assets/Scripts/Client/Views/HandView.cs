using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;

    [Header("Drag Settings")]
    [SerializeField] private BoxCollider dragValidArea; // 手牌安全区域，松手时如果不在这里就销毁
    [SerializeField] private float dragFollowDepth = 8f; // 鼠标拖拽时离摄像机多远

    private readonly List<GameObject> skillCardInstances = new();

    public IEnumerator AddCard(GameObject instance)
    {
        skillCardInstances.Add(instance);

        BindDragComponent(instance);

        yield return UpdateCardPositions(0.15f);
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
        DraggableSkillCard dragCard = instance.GetComponent<DraggableSkillCard>();
        if (dragCard == null)
            dragCard = instance.AddComponent<DraggableSkillCard>();

        dragCard.Init(this, dragValidArea, dragFollowDepth);
    }

    public IEnumerator UpdateCardPositions(float duration)
    {
        if (skillCardInstances.Count == 0)
            yield break;

        float cardSpacing = 1f / 50f;
        float firstCardPosition = 0.5f - (cardSpacing * (skillCardInstances.Count - 1) / 2f);
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < skillCardInstances.Count; i++)
        {
            GameObject card = skillCardInstances[i];
            if (card == null) continue;

            DraggableSkillCard dragCard = card.GetComponent<DraggableSkillCard>();
            if (dragCard != null && dragCard.IsDragging)
                continue; // 正在拖拽的牌不参与重排

            float p = firstCardPosition + i * cardSpacing;

            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 worldPos = splinePosition + transform.position + 0.01f * i * Vector3.back;

            Vector3 dir = new Vector3(26, 0, 0);
            Quaternion rotation = Quaternion.Euler(dir);

            // card.transform.DOKill();
            card.transform.DOMove(worldPos, duration);
            card.transform.DORotateQuaternion(rotation, duration);
        }

        yield return new WaitForSeconds(duration);
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
