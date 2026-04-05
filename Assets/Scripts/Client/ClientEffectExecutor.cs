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

        public static IEnumerator ValidateCard(Card card, MatchGateway gateway, int playerSlot, int cardIstanceId, Dictionary<int, List<int>> selectedSourceIds, Dictionary<int, List<int>> selectedTargetIds, Dictionary<int, bool> judgeList)
        {

            // 先检查能不能打这张牌（比如资源够不够，或者有没有特定的牌在场上等）
            int i = 0;
            while (i != -1)
            {
                EffectOp op = card.effects[i];
                DetermineParticipantsCommand cmd = new DetermineParticipantsCommand { playerId = playerSlot, effect = op };
                gateway.SendCommandServerRpc("DetermineParticipants", JsonConvert.SerializeObject(cmd), ClientGameState.playerSlot);
                yield return new WaitUntil(() =>
                    ClientGameState.GetServerGameStateDone &&
                    ClientEffectContext.GetServerCtxDone &&
                    ClientEffectContext.ChooseDone
                );
                ClientEffectContext.ChooseDone = false;

                if (!ClientEffectContext.IsCommandValid)
                {
                    Debug.Log($"[Client] Cannot play card instance {cardIstanceId} because of effect {op.type} validation failure");
                    ClearCardsToResolveCommand cmd2 = new ClearCardsToResolveCommand { playerId = ClientGameState.playerSlot, isPeekZone = false };
                    gateway.SendCommandServerRpc("ClearCardsToResolve", JsonConvert.SerializeObject(cmd2));
                    break;
                }
                else
                {
                    ClientEffectContext.IsCommandValid = true;  // IsCommandValid 会默认被重置为 false，所以这里先重置回 true，避免后续的效果执行流程受到影响
                    ClientEffectContext.IsValidateDone = true;
                }

                selectedSourceIds[i] = ClientEffectContext.Instance.selectedSourceIds;
                selectedTargetIds[i] = ClientEffectContext.Instance.selectedTargetIds;
                judgeList[i] = ClientEffectContext.JudgeResult;

                if (op.trueNode == -1 && op.falseNode == -1) break;
                else if (op.trueNode != -1 && op.falseNode == -1) i = op.trueNode;
                else if (op.trueNode == -1 && op.falseNode != -1) i = op.falseNode;
                else if (ClientEffectContext.JudgeResult) i= op.trueNode;
                else i = op.falseNode;
            }

        }

        public static IEnumerator ExecuteCard(Card card, MatchGateway gateway, int playerSlot, int cardInstanceId, Dictionary<int, List<int>> selectedSourceIds, Dictionary<int, List<int>> selectedTargetIds, Dictionary<int, bool> judgeList)
        {
            // 这里模拟一个需要玩家选择目标的效果执行流程：
            // 1. 客户端收到事件，解析出 Card 和 EffectOp（这里直接用参数传了）
            // 2. 弹 UI 让玩家选目标（这里直接等 0.1 秒模拟玩家选择）
            // 3. 玩家选好后，继续执行效果
            SpendActionPointCommand apCmd = new SpendActionPointCommand { playerId = playerSlot };
            gateway.SendCommandServerRpc("SpendActionPoint", JsonConvert.SerializeObject(apCmd), playerSlot);
            yield return new WaitUntil(() => CommandExecutionState<SpendActionPointCommand>.IsDone);

            int i = 0;
            while (i != -1)
            {
                EffectOp op = card.effects[i];
                ExecuteOp(op, gateway, ClientGameState.Instance, ClientEffectContext.Instance, selectedSourceIds[i], selectedTargetIds[i]);
                yield return new WaitUntil(() => ClientEffectContext.IsExecuteDone);
                ClientEffectContext.IsExecuteDone = false;

                if (op.trueNode == -1 && op.falseNode == -1) break;
                else if (op.trueNode != -1 && op.falseNode == -1) i = op.trueNode;
                else if (op.trueNode == -1 && op.falseNode != -1) i = op.falseNode;
                else if (judgeList[i]) i = op.trueNode;
                else i = op.falseNode;
            }
            ClearCardsToResolveCommand cmd2 = new ClearCardsToResolveCommand { playerId = ClientGameState.playerSlot, isPeekZone = false };
            gateway.SendCommandServerRpc("ClearCardsToResolve", JsonConvert.SerializeObject(cmd2));
        }

        private static void ExecuteOp(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> selectedSourceIds, List<int> selectedTargetIds) 
        {
            ClientEffectFunction clientEffectFunction = GameObject.Find("ClientEffectFunction").GetComponent<ClientEffectFunction>();
            clientEffectFunction.ExecuteOp(op, gateway, gameState, ctx, selectedSourceIds, selectedTargetIds);
        }
    }
}