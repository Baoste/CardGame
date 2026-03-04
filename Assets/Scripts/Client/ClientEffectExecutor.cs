using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

namespace Game.Domain
{
    public static class ClientEffectExecutor
    {
        public static IEnumerator ExcuteCard(Card card, MatchGateway gateway, int playerSlot, int cardIstanceId)
        {
            // 这里模拟一个需要玩家选择目标的效果执行流程：
            // 1. 客户端收到事件，解析出 Card 和 EffectOp（这里直接用参数传了）
            // 2. 弹 UI 让玩家选目标（这里直接等 0.1 秒模拟玩家选择）
            // 3. 玩家选好后，继续执行效果
            foreach (var op in card.effects)
            {
                ReadyToPlaySkillCardEffectCommand cmd = new ReadyToPlaySkillCardEffectCommand { playerId = playerSlot, effect = op };
                gateway.SendCommandServerRpc("ReadyToPlaySkillCardEffect", JsonConvert.SerializeObject(cmd), ClientGameState.playerSlot);
                yield return new WaitUntil(() =>
                    ClientGameState.GetServerGameStateDone &&
                    ClientEffectContext.GetServerCtxDone &&
                    ClientEffectContext.ChooseDone
                );
                ClientEffectContext.ChooseDone = false;
                ExecuteOp(op, gateway, ClientGameState.Instance, ClientEffectContext.Instance);
                yield return new WaitUntil(() => ClientEffectContext.IsExecuteDone);
                ClientEffectContext.IsExecuteDone = false;
            }

            // TODO: 这里直接丢弃了，后续可能需要根据效果来决定是否丢弃
            //DiscardCardCommand discardCmd = new DiscardCardCommand { playerId = playerSlot, instanceId = cardIstanceId };
            //gateway.SendCommandServerRpc("DiscardCard", JsonConvert.SerializeObject(discardCmd));
        }

        private static void ExecuteOp(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx) 
        {
            ClientEffectFunction clientEffectFunction = GameObject.Find("ClientEffectFunction").GetComponent<ClientEffectFunction>();
            switch (op.type)
            {
                case EffectType.DrawPoint:
                    clientEffectFunction.DrawPointCards(op, gateway, gameState, ctx);
                    break;

                case EffectType.Discard:
                    clientEffectFunction.DiscardCards(op, gateway, gameState, ctx);
                    break;

                case EffectType.ModifyPoint:
                    clientEffectFunction.ModifyPoint(op, gateway, gameState, ctx);
                    break;

                case EffectType.Move:
                    //Draw(op, state, ctx);
                    break;
            }
        }
    }
}