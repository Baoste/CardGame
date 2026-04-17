using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToDrawPointCard : MonoBehaviour, IMouseClick
{
    public void MouseClick()
    {
        if (ClientEffectContext.isExecutingSkillCard) return;
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
        {
            Debug.Log("不是你的回合");
            return;
        }

        if (ClientGameState.Instance.CurrentPlayerId != -1 && ClientGameState.Instance.CurrentPlayerId == ClientGameState.playerSlot)
        {
            StartCoroutine(DrawPointCard());
        }
    }

    private IEnumerator DrawPointCard()
    {
        DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("DrawPointCard", JsonConvert.SerializeObject(cmd));

        yield break;
    }
}
