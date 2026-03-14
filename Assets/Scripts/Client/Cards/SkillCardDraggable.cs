using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardDraggable : MonoBehaviour
{
    private Camera cam;
    private bool isDragging;
    private Plane dragPlane;

    private int cardId;
    private int instanceId;

    public bool IsDragging => isDragging;
    public bool isExecuting;

    private SkillCardInstance instance;
    private SkillCardMouseTilt mouseTilt;

    public void Init(int cardId, int instanceId)
    {
        this.cardId = cardId;
        this.instanceId = instanceId;
        cam = Camera.main;
        instance = GetComponent<SkillCardInstance>();
        mouseTilt = GetComponent<SkillCardMouseTilt>();
        isExecuting = false;
    }

    private void OnMouseDown()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        isDragging = true;
        transform.DOKill();

        Vector3 planeNormal = SceneViewManager.myHandView.transform.rotation * Vector3.up;
        dragPlane = new Plane(planeNormal, SceneViewManager.myHandView.transform.position);

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            transform.position = mouseWorld;
            // 转到 handView 的坐标系下
            Vector3 mouseLocal = SceneViewManager.myHandView.transform.InverseTransformDirection(mouseWorld);
            mouseTilt.InitTilt(mouseLocal);
        }
    }

    private void OnMouseDrag()
    {
        if (!isDragging || cam == null) return;

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            transform.position = mouseWorld;
            Vector3 mouseLocal = SceneViewManager.myHandView.transform.InverseTransformDirection(mouseWorld);
            mouseTilt.Tilt(mouseLocal);

            //// 拖拽时仍然朝向摄像机
            //Vector3 dir = cam.transform.position - transform.position;
            //Quaternion rotation = Quaternion.LookRotation(-dir);
            //transform.rotation = rotation;

            // ===== 新增：检测是否离开合法区域 =====
            bool outside = SceneViewManager.myHandView != null && SceneViewManager.myHandView.IsOutsideValidArea(transform.position);

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

        bool shouldRemove = SceneViewManager.myHandView != null && SceneViewManager.myHandView.IsOutsideValidArea(transform.position);

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
        isExecuting = true;

        // TODO: Debug
        //yield return StartCoroutine(SceneViewManager.myExecuteCardView.MoveToFallPosition(gameObject));
        //yield return new WaitForSeconds(1f);
        //yield return StartCoroutine(SceneViewManager.myExecuteCardView.MoveToExecutePosition(gameObject));
        //yield return new WaitForSeconds(0.5f);
        //yield return StartCoroutine(SceneViewManager.myHandView.RemoveCard(gameObject, ClientGameState.playerSlot));
        //yield break;

        // Start Executing
        yield return StartCoroutine(ClientEffectExecutor.ValidateActionPoint(ClientGameState.gateway, ClientGameState.playerSlot));
        if (!CommandExecutionState<ValidateActionPointCommand>.Success)
        {
            transform.localScale = Vector3.one * instance.localScaleFactor;
            Debug.Log("没有足够的行动点");
            StartCoroutine(ReturnToHand());
            yield break;
        }

        yield return StartCoroutine(SceneViewManager.myExecuteCardView.MoveToFallPosition(gameObject));
        Dictionary<int, List<int>> selectedSourceIds = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> selectedTargetIds = new Dictionary<int, List<int>>();

        Card card = CardDatabase.Get(cardId);
        StartCoroutine(ClientEffectExecutor.ValidateCard(card, ClientGameState.gateway, ClientGameState.playerSlot, instanceId, selectedSourceIds, selectedTargetIds));
        yield return new WaitUntil(() => ClientEffectContext.IsValidateDone);

        if (!ClientEffectContext.IsCommandValid)
        {
            transform.localScale = Vector3.one * instance.localScaleFactor;
            Debug.Log("你不能打出这张牌");
            StartCoroutine(ReturnToHand());
        }
        else
        {
            yield return StartCoroutine(SceneViewManager.myExecuteCardView.MoveToExecutePosition(gameObject));
            StartCoroutine(ClientEffectExecutor.ExecuteCard(card, ClientGameState.gateway, ClientGameState.playerSlot, instanceId, selectedSourceIds, selectedTargetIds));
            // yield return StartCoroutine(SceneViewManager.myHandView.RemoveCard(gameObject, ClientGameState.playerSlot));
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
        yield return SceneViewManager.myHandView.UpdateCardPositions(0.15f, ClientGameState.playerSlot);
    }
}
