using DG.Tweening;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToCallOrFold : MonoBehaviour, IMouseDown, IMouseUp
{
    public bool isCall;
    [HideInInspector] public int betCount;

    private Tween rotateTween;
    private Transform parent;

    private void Start()
    {
        parent = transform.parent;
    }

    public void MouseDown()
    {
        rotateTween?.Kill();

        rotateTween = DOTween.Sequence()
            .Append(parent.DORotate(
                new Vector3(0, 5f, 0),
                0.15f,
                RotateMode.LocalAxisAdd
            ).SetEase(Ease.OutCubic))
            .Append(parent.DORotate(
                parent.localEulerAngles,
                0.35f,
                RotateMode.Fast
            ).SetEase(Ease.OutBack));
    }

    public void MouseUp()
    {
        SendCallOrFoldCmd(isCall);
    }

    private void SendCallOrFoldCmd(bool isCall)
    {
        ConfirmBetCommand cmd = new ConfirmBetCommand { playerId = ClientGameState.playerSlot, isCall = isCall, betCount = betCount };
        ClientGameState.gateway.SendCommandServerRpc("ConfirmBet", JsonConvert.SerializeObject(cmd));
    }
}
