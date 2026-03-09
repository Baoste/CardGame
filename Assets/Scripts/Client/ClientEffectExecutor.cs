using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Domain
{
    public static class ClientEffectExecutor
    {
        public static IEnumerator ValidateCard(Card card, MatchGateway gateway, int playerSlot, int cardIstanceId, Dictionary<int, List<int>> selectedSourceIds, Dictionary<int, List<int>> selectedTargetIds)
        {
            // 先检查能不能打这张牌（比如资源够不够，或者有没有特定的牌在场上等）
            for (int i = 0; i < card.effects.Count; i++)
            {
                EffectOp op = card.effects[i];

                // TODO: 暂时 Move 效果的验证，因为 Move 效果的验证比较特殊，后续再单独处理
                if (op.type == EffectType.Move && op.target.participantType == ParticipantType.CardsToResolve)
                {
                    // excute move effect
                }
                yield return new WaitForSecondsRealtime(2f);

                DetermineParticipantsCommand cmd = new DetermineParticipantsCommand { playerId = playerSlot, effect = op };
                gateway.SendCommandServerRpc("DetermineParticipants", JsonConvert.SerializeObject(cmd), ClientGameState.playerSlot);
                yield return new WaitUntil(() =>
                    ClientGameState.GetServerGameStateDone &&
                    ClientEffectContext.GetServerCtxDone &&
                    ClientEffectContext.ChooseDone
                );
                ClientEffectContext.ChooseDone = false;

                // TODO: 暂时 Move 效果的验证，因为 Move 效果的验证比较特殊，后续再单独处理
                if (op.type == EffectType.Move && op.target.participantType == ParticipantType.CardsToResolve)
                {
                    MoveValid(op, gateway, ClientGameState.Instance, ClientEffectContext.Instance);
                }
                yield return new WaitForSecondsRealtime(2f);

                if (!ClientEffectContext.IsCommandValid)
                {
                    Debug.Log($"[Client] Cannot play card instance {cardIstanceId} because of effect {op.type} validation failure");
                    break;
                }
                else
                {
                    ClientEffectContext.IsCommandValid = true;  // IsCommandValid 会默认被重置为 false，所以这里先重置回 true，避免后续的效果执行流程受到影响
                }

                selectedSourceIds[i] = ClientEffectContext.Instance.selectedSourceIds;
                selectedTargetIds[i] = ClientEffectContext.Instance.selectedTargetIds;
            }
            ClientEffectContext.IsValidateDone = true;
        }

        public static IEnumerator ExecuteCard(Card card, MatchGateway gateway, int playerSlot, int cardIstanceId, Dictionary<int, List<int>> selectedSourceIds, Dictionary<int, List<int>> selectedTargetIds)
        {
            // 这里模拟一个需要玩家选择目标的效果执行流程：
            // 1. 客户端收到事件，解析出 Card 和 EffectOp（这里直接用参数传了）
            // 2. 弹 UI 让玩家选目标（这里直接等 0.1 秒模拟玩家选择）
            // 3. 玩家选好后，继续执行效果
            for (int i = 0; i < card.effects.Count; i++)
            {
                EffectOp op = card.effects[i];
                ExecuteOp(op, gateway, ClientGameState.Instance, ClientEffectContext.Instance, selectedSourceIds[i], selectedTargetIds[i]);
                yield return new WaitUntil(() => ClientEffectContext.IsExecuteDone);
                ClientEffectContext.IsExecuteDone = false;
            }

            // TODO: 这里直接丢弃了，后续可能需要根据效果来决定是否丢弃
            Debug.Log($"[Client] Discard skill card instance {cardIstanceId}");
            DiscardCardCommand discardCmd = new DiscardCardCommand { playerId = playerSlot, instanceId = cardIstanceId };
            gateway.SendCommandServerRpc("DiscardCard", JsonConvert.SerializeObject(discardCmd));
        }

        private static void ExecuteOp(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> selectedSourceIds, List<int> selectedTargetIds) 
        {
            ClientEffectFunction clientEffectFunction = GameObject.Find("ClientEffectFunction").GetComponent<ClientEffectFunction>();
            switch (op.type)
            {
                case EffectType.DrawPoint:
                    clientEffectFunction.DrawPointCards(op, gateway, gameState, ctx);
                    break;

                case EffectType.DrawSkill:
                    clientEffectFunction.DrawSkillCards(op, gateway, gameState, ctx);
                    break;

                case EffectType.Discard:
                    clientEffectFunction.DiscardCards(op, gateway, gameState, ctx, selectedTargetIds);
                    break;

                case EffectType.ModifyPoint:
                    clientEffectFunction.ModifyPoint(op, gateway, gameState, ctx, selectedTargetIds);
                    break;

                case EffectType.Move:
                    clientEffectFunction.MoveCards(op, gateway, gameState, ctx, selectedSourceIds);
                    break;
            }
        }

        private static void MoveValid(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx)
        {
            ClientEffectFunction clientEffectFunction = GameObject.Find("ClientEffectFunction").GetComponent<ClientEffectFunction>();
            clientEffectFunction.MoveValid(op, gateway, gameState, ctx);
        }
    }
}