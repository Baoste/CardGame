
using Game.Domain;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using System.ComponentModel;

public class SkillCard : MonoBehaviour
{
    #region State
    [HideInInspector] public SkillCardStateMachine stateMachine;
    [HideInInspector] public SkillCardInDeckState inDeckState;
    [HideInInspector] public SkillCardInHandState inHandState;
    [HideInInspector] public SkillCardDragState dragState;
    [HideInInspector] public SkillCardReadyFallState readyFallState;
    [HideInInspector] public SkillCardExecuteState executeState;
    #endregion

    #region Component
    // public Animator animator { get; private set; }
    [HideInInspector] public Outline outlineControl;
    [HideInInspector] public SkillCardInstance instance;
    #endregion

    private bool isOpponent;


    private void Awake()
    {
        stateMachine = new SkillCardStateMachine();
        inDeckState = new SkillCardInDeckState(stateMachine, this, "isInDeck");
        inHandState = new SkillCardInHandState(stateMachine, this, "isInHand");
        dragState = new SkillCardDragState(stateMachine, this, "isDrag");
        readyFallState = new SkillCardReadyFallState(stateMachine, this, "isReadyFall");
        executeState = new SkillCardExecuteState(stateMachine, this, "isExecute");
    }

    private void Start()
    {
        // animator = GetComponentInChildren<Animator>();
        instance = GetComponent<SkillCardInstance>();
        outlineControl = GetComponent<Outline>();

        stateMachine.Initialize(inDeckState);
    }

    public void SetIsOpponent(bool isOpponent)
    {
        this.isOpponent = isOpponent;
    }

    public void UpdateCardPosition()
    {
        StartCoroutine(_UpdateCardPosition());
    }

    private IEnumerator _UpdateCardPosition()
    {
        yield return SceneViewManager.myHandView.UpdateCardPositions(0.15f);
        yield return SceneViewManager.opponentHandView.UpdateCardPositions(0.15f);
    }

    public void StartExecuteCard()
    {
        StartCoroutine(_StartExecuteCard());
    }   
    
    private IEnumerator _StartExecuteCard()
    {
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
        {
            Debug.Log("不是你的回合");
            stateMachine.ChangeState(inHandState);
            yield break;
        }

        // Start Executing
        // 准备执行技能
        // TODO: 需要广播动画
        PlayAnimationCommand animCmd = new PlayAnimationCommand { playerId = ClientGameState.playerSlot, animType = AnimationType.MoveToFallPosition, instanceId = instance.instanceId };
        ClientGameState.gateway.SendCommandServerRpc("PlayAnimation", JsonConvert.SerializeObject(animCmd));
        CommandExecutionState<PlayAnimationCommand>.IsDone = false;
        yield return new WaitUntil(() => CommandExecutionState<PlayAnimationCommand>.IsDone);

        // 执行
        StartExecuteSkillCommand cmd = new StartExecuteSkillCommand { playerId = ClientGameState.playerSlot, instanceId = instance.instanceId };
        ClientGameState.gateway.SendCommandServerRpc("StartExecuteSkill", JsonConvert.SerializeObject(cmd));
    }

    public void MoveToFallPosition()
    {
        if (isOpponent)
            StartCoroutine(SceneViewManager.opponentExecuteCardView.MoveToFallPosition(gameObject));
        else
            StartCoroutine(SceneViewManager.myExecuteCardView.MoveToFallPosition(gameObject));
    }

    public void MoveToExecutePosition()
    {
        if (isOpponent)
            StartCoroutine(SceneViewManager.opponentExecuteCardView.MoveToExecutePosition(gameObject));
        else
            StartCoroutine(SceneViewManager.myExecuteCardView.MoveToExecutePosition(gameObject));
    }
}
