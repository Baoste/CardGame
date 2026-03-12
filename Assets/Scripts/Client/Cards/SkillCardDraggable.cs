using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardDraggable : MonoBehaviour
{
    private HandView handView;

    private Camera cam;
    private bool isDragging;
    private Plane dragPlane;

    private int cardId;
    private int instanceId;

    public bool IsDragging => isDragging;

    private SkillCardInstance instance;
    private SkillCardMouseTilt mouseTilt;

    public void Init(HandView handView, int cardId, int instanceId)
    {
        this.handView = handView;
        this.cardId = cardId;
        this.instanceId = instanceId;
        cam = Camera.main;
        instance = GetComponent<SkillCardInstance>();
        mouseTilt = GetComponent<SkillCardMouseTilt>();
    }

    private void OnMouseDown()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        isDragging = true;
        transform.DOKill();

        Vector3 planeNormal = handView.transform.rotation * Vector3.up;
        dragPlane = new Plane(planeNormal, handView.transform.position);

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            transform.position = mouseWorld;
            // 转到 handView 的坐标系下
            Vector3 mouseLocal = handView.transform.InverseTransformDirection(mouseWorld);
            mouseTilt.InitTilt(mouseLocal);
        }
    }

    private void OnMouseDrag()
    {
        if (!isDragging || cam == null) return;

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            transform.position = mouseWorld;
            Vector3 mouseLocal = handView.transform.InverseTransformDirection(mouseWorld);
            mouseTilt.Tilt(mouseLocal);

            //// 拖拽时仍然朝向摄像机
            //Vector3 dir = cam.transform.position - transform.position;
            //Quaternion rotation = Quaternion.LookRotation(-dir);
            //transform.rotation = rotation;

            // ===== 新增：检测是否离开合法区域 =====
            bool outside = handView != null && handView.IsOutsideValidArea(transform.position);

            if (outside)
            {
                instance.meshRenderer.sharedMaterial = instance.outsideAreaMaterial;
                transform.localScale = Vector3.one * instance.localScaleFactor * 1.1f;
            }
            else
            {
                instance.meshRenderer.sharedMaterial = instance.defaultMaterial;
                transform.localScale = Vector3.one * instance.localScaleFactor;
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
            StartCoroutine(ExecuteCard());
        }
        else
        {
            StartCoroutine(ReturnToHand());
        }
    }

    IEnumerator ExecuteCard()
    {
        // TODO: 这里应该放一些特效
        transform.localScale = Vector3.one * 0.01f;

        // TODO: Debug
        StartCoroutine(handView.RemoveCard(gameObject, ClientGameState.playerSlot));
        yield break;

        Dictionary<int, List<int>> selectedSourceIds = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> selectedTargetIds = new Dictionary<int, List<int>>();

        Card card = CardDatabase.Get(cardId);
        StartCoroutine(ClientEffectExecutor.ValidateCard(card, ClientGameState.gateway, ClientGameState.playerSlot, instanceId, selectedSourceIds, selectedTargetIds));
        yield return new WaitUntil(() => ClientEffectContext.IsValidateDone);

        if (!ClientEffectContext.IsCommandValid)
        {
            transform.localScale = Vector3.one * instance.localScaleFactor;
            StartCoroutine(ReturnToHand());
        }
        else
        {
            StartCoroutine(ClientEffectExecutor.ExecuteCard(card, ClientGameState.gateway, ClientGameState.playerSlot, instanceId, selectedSourceIds, selectedTargetIds));
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
        mouseTilt.ResetBaseRotation();
        if (handView != null)
            yield return handView.UpdateCardPositions(0.15f, ClientGameState.playerSlot);
    }
}
