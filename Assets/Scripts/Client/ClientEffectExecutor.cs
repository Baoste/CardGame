using Game.Server;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Game.Domain
{
    public static class ClientEffectExecutor
    {
        //public static void ExecuteCardReady(Card card, MatchGateway gateway, int playerSlot)
        //{
        //    foreach (var op in card.effects)
        //    {
        //        ReadyToPlaySkillCardEffectCommand cmd = new ReadyToPlaySkillCardEffectCommand { playerId = playerSlot, effect = op };
        //        gateway.SendCommandServerRpc("ReadyToPlaySkillCardEffect", JsonUtility.ToJson(cmd));
        //    }
        //}

        //public static void ExecuteCard(Card card, MatchGateway gateway, List<int> targetIds)
        //{
        //    foreach (var op in card.effects)
        //        ExecuteOp(op, gateway, ClientGameState.Instance, ClientEffectContext.Instance, targetIds);
        //}

        public static void ExecuteOp(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> targetIds)
        {
            switch (op.type)
            {
                case EffectType.DrawCards:
                    ClientEffectFunction.DrawCards(op, gateway, gameState, ctx, targetIds);
                    break;

                case EffectType.DiscardCards:
                    //DrawCards(op, state, ctx);
                    break;

                case EffectType.ModifyCardPoints:
                    //ModifyCardPoints(op, state, ctx);
                    break;

                case EffectType.MoveCards:
                    //DrawCards(op, state, ctx);
                    break;
            }
        }
    }
}