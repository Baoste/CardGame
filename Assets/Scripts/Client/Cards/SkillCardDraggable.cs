using DG.Tweening;
using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
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
    public bool executed = false;

    private SkillCardInstance instance;
    private SkillCardMouseTilt mouseTilt;
    private Outline outlineControl;

    public void Init(int cardId, int instanceId)
    {
        this.cardId = cardId;
        this.instanceId = instanceId;
        cam = Camera.main;
        instance = GetComponent<SkillCardInstance>();
        mouseTilt = GetComponent<SkillCardMouseTilt>();
        outlineControl = GetComponent<Outline>();
    }

    private void OnMouseDown()
    {
        if (executed) return;
        if (ClientEffectContext.isExecutingSkillCard) return;
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
        if (executed) return;
        if (ClientEffectContext.isExecutingSkillCard) return;
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
                outlineControl.OutlineColor = outlineControl.outAreaColor;
                transform.localScale = Vector3.one * instance.localScaleFactor;
            }
            else
            {
                // instance.meshRenderer.sharedMaterial = instance.defaultMaterial;
                outlineControl.OutlineColor = outlineControl.defaultColor;
                transform.localScale = Vector3.one * instance.localScaleFactor;
            }
        }
    }

    private void OnMouseUp()
    {
        if (executed) return;
        if (ClientEffectContext.isExecutingSkillCard) return;
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
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
        {
            Debug.Log("不是你的回合");
            StartCoroutine(ReturnToHand());
            yield break;
        }

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
        ClientEffectContext.isExecutingSkillCard = true;

        // 准备执行技能
        // TODO: 需要广播动画
        PlayAnimationCommand animCmd = new PlayAnimationCommand { playerId = ClientGameState.playerSlot, animType = AnimationType.MoveToFallPosition, instanceId = instanceId};
        ClientGameState.gateway.SendCommandServerRpc("PlayAnimation", JsonConvert.SerializeObject(animCmd));
        CommandExecutionState<PlayAnimationCommand>.IsDone = false;
        yield return new WaitUntil(() => CommandExecutionState<PlayAnimationCommand>.IsDone);

        Dictionary<int, List<int>> selectedSourceIds = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> selectedTargetIds = new Dictionary<int, List<int>>();
        Dictionary<int, bool> judgeList = new Dictionary<int, bool>();

        Card card = CardDatabase.Get(cardId);
        yield return StartCoroutine(ClientEffectExecutor.ValidateCard(card, ClientGameState.gateway, ClientGameState.playerSlot, instanceId, selectedSourceIds, selectedTargetIds, judgeList));

        if (!ClientEffectContext.IsValidateDone)
        {
            // 执行技能失败
            Debug.Log("你不能打出这张牌");
            transform.localScale = Vector3.one * instance.localScaleFactor;
            ClientEffectContext.isExecutingSkillCard = false;
            // TODO: 需要广播动画
            animCmd = new PlayAnimationCommand { playerId = ClientGameState.playerSlot, animType = AnimationType.ReturnToHand, instanceId = instanceId };
            ClientGameState.gateway.SendCommandServerRpc("PlayAnimation", JsonConvert.SerializeObject(animCmd));
            yield return new WaitUntil(() => CommandExecutionState<PlayAnimationCommand>.IsDone);
        }
        else
        {
            // 如果执行了一个op之后才失败了
            if (!ClientEffectContext.IsCommandValid)
            {
                // TODO: 暂时的，以后要改成消失动画
                transform.localScale = Vector3.zero;
                transform.position = new Vector3(0, 0, 100);
                ClientEffectContext.isExecutingSkillCard = false;
                StartCoroutine(SceneViewManager.myHandView.RemoveCard(gameObject));
                StartCoroutine(SceneViewManager.opponentHandView.RemoveCard(gameObject));
                yield break;
            }
            // 执行
            // TODO: 需要广播动画
            animCmd = new PlayAnimationCommand { playerId = ClientGameState.playerSlot, animType = AnimationType.MoveToExecutePosition, instanceId = instanceId };
            ClientGameState.gateway.SendCommandServerRpc("PlayAnimation", JsonConvert.SerializeObject(animCmd));
            CommandExecutionState<PlayAnimationCommand>.IsDone = false;
            yield return new WaitUntil(() => CommandExecutionState<PlayAnimationCommand>.IsDone);

            yield return StartCoroutine(ClientEffectExecutor.ExecuteCard(card, ClientGameState.gateway, ClientGameState.playerSlot, instanceId, selectedSourceIds, selectedTargetIds, judgeList));
            ClientEffectContext.isExecutingSkillCard = false;
            // 这里直接丢弃
            Debug.Log($"[Client] Discard skill card instance {instanceId}");
            DiscardCardCommand discardCmd = new DiscardCardCommand { playerId = ClientGameState.playerSlot, instanceId = instanceId };
            ClientGameState.gateway.SendCommandServerRpc("DiscardCard", JsonConvert.SerializeObject(discardCmd));
            //Destroy(gameObject);
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

    public IEnumerator ReturnToHand()
    {
        ClientEffectContext.isExecutingSkillCard = false;
        mouseTilt.ResetBaseRotation();
        instance.meshRenderer.sharedMaterial = instance.defaultMaterial;
        transform.localScale = Vector3.one * instance.localScaleFactor;
        yield return SceneViewManager.myHandView.UpdateCardPositions(0.15f);
        yield return SceneViewManager.opponentHandView.UpdateCardPositions(0.15f);
    }
}
