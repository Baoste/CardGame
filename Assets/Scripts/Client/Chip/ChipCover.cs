using DG.Tweening;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipCover : MonoBehaviour, IMouseDown
{
    public void MouseDown()
    {
        if (ClientGameState.playerSlot != ClientGameState.Instance.punterId)
            return;
        if (SceneViewManager.myChipView.chipsPlaced.Count < 1)
            return;

        ConfirmBetCommand cmd = new ConfirmBetCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("ConfirmBet", JsonConvert.SerializeObject(cmd));
    }
}
