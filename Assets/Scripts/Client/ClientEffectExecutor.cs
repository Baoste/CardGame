using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Domain
{
    public static class ClientEffectExecutor
    {
        public static IEnumerator ValidateActionPoint(MatchGateway gateway, int playerSlot, int apCount)
        {
            // 检查行动点数
            ValidateActionPointCommand apCmd = new ValidateActionPointCommand { playerId = playerSlot, apCount = apCount };
            gateway.SendCommandServerRpc("ValidateActionPoint", JsonConvert.SerializeObject(apCmd), ClientGameState.playerSlot);
            yield return new WaitUntil(() =>
                CommandExecutionState<ValidateActionPointCommand>.IsDone
            );
        }
    }
}