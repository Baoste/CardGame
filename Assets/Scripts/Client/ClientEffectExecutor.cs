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

        public static IEnumerator ExecuteCard(MatchGateway gateway, int playerSlot, int cardInstanceId)
        {
            SpendActionPointCommand apCmd = new SpendActionPointCommand { playerId = playerSlot };
            gateway.SendCommandServerRpc("SpendActionPoint", JsonConvert.SerializeObject(apCmd), playerSlot);
            yield return new WaitUntil(() => CommandExecutionState<SpendActionPointCommand>.IsDone);

            StartExecuteSkillCommand cmd = new StartExecuteSkillCommand { playerId = playerSlot, instanceId = cardInstanceId };
            gateway.SendCommandServerRpc("StartExecuteSkill", JsonConvert.SerializeObject(cmd));
            //yield return new WaitUntil(() => CommandExecutionState<StartExecuteSkillCommand>.IsDone);

            //ClearCardsToResolveCommand cmd2 = new ClearCardsToResolveCommand { playerId = ClientGameState.playerSlot, isPeekZone = false };
            //gateway.SendCommandServerRpc("ClearCardsToResolve", JsonConvert.SerializeObject(cmd2));
        }
    }
}