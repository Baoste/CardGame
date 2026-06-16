using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipController : MonoBehaviour
{
    #region State
    [HideInInspector] public ChipStateMachine stateMachine;
    [HideInInspector] public ChipInTrayState inTrayState;
    [HideInInspector] public ChipDragState dragState;
    [HideInInspector] public ChipPlacedState placedState;
    [HideInInspector] public ChipSelectedState selectedState;
    [HideInInspector] public ChipDiscardState discardState;
    #endregion

    [HideInInspector] public Outline outlineControl;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider col;
    [HideInInspector] public Transform originalTransform;

    [HideInInspector] public int instanceId;

    private void Awake()
    {
        stateMachine = new ChipStateMachine();
        inTrayState = new ChipInTrayState(stateMachine, this, "isInTray");
        dragState = new ChipDragState(stateMachine, this, "isDrag");
        placedState = new ChipPlacedState(stateMachine, this, "isPlaced");
        selectedState = new ChipSelectedState(stateMachine, this, "isSelected");
        discardState = new ChipDiscardState(stateMachine, this, "isDiscard");
    }
    private void Start()
    {
        // animator = GetComponentInChildren<Animator>();
        outlineControl = GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();
        col = transform.Find("Chip").GetComponent<Collider>();

        stateMachine.Initialize(inTrayState);
    }

    public void StartExecuteChip()
    {
        StartCoroutine(_StartExecuteChip());
    }

    private IEnumerator _StartExecuteChip()
    {
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
        {
            Debug.Log("不是你的回合");
            stateMachine.ChangeState(inTrayState);
            yield break;
        }

        if (SceneViewManager.opponentChipView.chipsInTray.Count < 1)
        {
            Debug.Log("对方没有筹码了");
            stateMachine.ChangeState(inTrayState);
            yield break;
        }

        // Start Executing
        stateMachine.ChangeState(placedState);

        Place1BetCommand cmd = new Place1BetCommand { playerId = ClientGameState.playerSlot, instanceId = instanceId };
        ClientGameState.gateway.SendCommandServerRpc("Place1Bet", JsonConvert.SerializeObject(cmd));
    }
}
