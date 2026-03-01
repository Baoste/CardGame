using Game.Server;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Domain
{
    public static class ClientEffectFunction
    {
        public static void DrawCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> targetIds)
        {
            int selectedCardId = ctx.selectedCards.Count > 0 ? ctx.selectedCards[0] : -1;
            int drawNum = op.value.Evaluate(gameState, ctx, selectedCardId);
            for (int i = 0; i < drawNum; i++)
            {
                // TODO: PlaySkillCardEffectWithTargetCommand cmd
                DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = ctx.caster };
                gateway.SendCommandServerRpc("DrawPointCard", JsonUtility.ToJson(cmd));
            }
        }

        public static void ModifyCardPoints(EffectOp op, GameState state, EffectContext ctx)
        {
            foreach (var card in ctx.selectedCards)
            {
                // TODO: send modify point event
                // card.Points += op.value.Evaluate(state, ctx, card);
            }
        }
    }
}