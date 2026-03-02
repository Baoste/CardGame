using Game.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Domain
{
    public class ClientEffectFunction : MonoBehaviour
    {
        public void DrawCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx)
        {
            int selectedCardId = ctx.selectedTargetIds.Count > 0 ? ctx.selectedTargetIds[0] : -1;
            int drawNum = op.value.Evaluate(gameState, ctx, selectedCardId);
            for (int i = 0; i < drawNum; i++)
            {
                StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, () =>
                {
                    DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = ctx.caster };
                    gateway.SendCommandServerRpc("DrawPointCard", JsonUtility.ToJson(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }
        
        //public void DrawCard(MatchGateway gateway, GameState gameState, EffectContext ctx)
        //{
        //    DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = ctx.caster };
        //    gateway.SendCommandServerRpc("DrawPointCard", JsonUtility.ToJson(cmd));
        //}

        //public static void ModifyCardPoints(EffectOp op, GameState state, EffectContext ctx)
        //{
        //    foreach (var card in ctx.selectedCards)
        //    {
        //        // TODO: send modify point event
        //        // card.Points += op.value.Evaluate(state, ctx, card);
        //    }
        //}

        public IEnumerator ValidateCommand(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, Action onSuccess)
        {
            PlaySkillCardEffectWithTargetCommand cmd = new PlaySkillCardEffectWithTargetCommand
            {
                playerId = ctx.caster,
                effect = op,
                selectedSourceIds = ctx.selectedSourceIds,
                selectedTargetIds = ctx.selectedTargetIds
            };
            gateway.SendCommandServerRpc("PlaySkillCardEffectWithTarget", JsonUtility.ToJson(cmd));
            yield return new WaitUntil(() => ClientEffectContext.IsValidateDone);
            ClientEffectContext.IsValidateDone = false;

            if (!ClientEffectContext.IsCommandValid)
            {
                Debug.Log($"[Client] Validate failed");
                yield break;
            }
            onSuccess.Invoke();
        }
    }
}