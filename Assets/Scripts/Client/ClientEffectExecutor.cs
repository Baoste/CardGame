using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Domain
{
    public static class ClientEffectExecutor
    {
        public static IEnumerator ValidateActionPoint(MatchGateway gateway, int playerSlot)
        {
            // 检查行动点数
            ValidateActionPointCommand apCmd = new ValidateActionPointCommand { playerId = playerSlot };
            gateway.SendCommandServerRpc("ValidateActionPoint", JsonConvert.SerializeObject(apCmd), ClientGameState.playerSlot);
            yield return new WaitUntil(() =>
                CommandExecutionState<ValidateActionPointCommand>.IsDone
            );
        }
    }
}