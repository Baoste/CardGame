using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class DraggableSkillCard : MonoBehaviour
{
    private HandView handView;
    private BoxCollider validArea;
    private float dragFollowDepth = 8f;

    private Camera cam;
    private bool isDragging;
    private Vector3 dragOffset;
    private Plane dragPlane;

    public bool IsDragging => isDragging;

    public void Init(HandView handView, BoxCollider validArea, float dragFollowDepth)
    {
        this.handView = handView;
        this.validArea = validArea;
        this.dragFollowDepth = dragFollowDepth;
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        isDragging = true;
        transform.DOKill();

        // 用一个和摄像机朝向近似平行的拖拽平面
        dragPlane = new Plane(-cam.transform.forward, transform.position);

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            dragOffset = transform.position - mouseWorld;
        }
        else
        {
            dragOffset = Vector3.zero;
        }
    }

    private void OnMouseDrag()
    {
        if (!isDragging || cam == null) return;

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            Vector3 targetPos = mouseWorld + dragOffset;
            transform.position = targetPos;

            // 拖拽时仍然朝向摄像机
            Vector3 dir = cam.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(-dir);
            transform.rotation = rotation;

            // ===== 新增：检测是否离开合法区域 =====
            bool outside = handView != null && handView.IsOutsideValidArea(transform.position);

            if (outside)
            {
                transform.localScale = Vector3.one * 0.4f;
            }
            else
            {
                transform.localScale = Vector3.one * 0.3f;
            }
        }
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        bool shouldRemove = handView != null && handView.IsOutsideValidArea(transform.position);

        if (shouldRemove)
        {
            Card card = CardDatabase.Get(999);
            // StartCoroutine(ClientEffectExecutor.ExecuteCard(card, ClientGameState.gateway, ClientGameState.playerSlot, -1));
            StartCoroutine(RemoveAndDestroy());
        }
        else
        {
            StartCoroutine(ReturnToHand());
        }
    }

    private bool TryGetMouseWorldPosition(out Vector3 worldPos)
    {
        worldPos = Vector3.zero;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            worldPos = ray.GetPoint(enter);
            return true;
        }

        return false;
    }

    private IEnumerator ReturnToHand()
    {
        if (handView != null)
            yield return handView.UpdateCardPositions(0.15f);
    }

    private IEnumerator RemoveAndDestroy()
    {
        if (handView != null)
            yield return handView.RemoveCard(gameObject);

        Destroy(gameObject);
    }
}
