using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToDrawSkillCard : MonoBehaviour, IMouseClick
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
            StartCoroutine(DrawSkillCard());
        }
    }

    private IEnumerator DrawSkillCard()
    {
        yield return StartCoroutine(ClientEffectExecutor.ValidateActionPoint(ClientGameState.gateway, ClientGameState.playerSlot, 1));
        if (!CommandExecutionState<ValidateActionPointCommand>.Success)
        {
            Debug.Log("没有足够的行动点");
            yield break;
        }

        SpendActionPointCommand apCmd = new SpendActionPointCommand { playerId = ClientGameState.playerSlot, apCount = 1 };
        ClientGameState.gateway.SendCommandServerRpc("SpendActionPoint", JsonConvert.SerializeObject(apCmd), ClientGameState.playerSlot);
        yield return new WaitUntil(() => CommandExecutionState<SpendActionPointCommand>.IsDone);

        DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("DrawSkillCard", JsonConvert.SerializeObject(cmd));
        yield break;
    }
}
