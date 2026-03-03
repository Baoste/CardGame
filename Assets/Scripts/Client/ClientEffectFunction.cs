using Game.Server;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Domain
{
    public class ClientEffectFunction : MonoBehaviour
    {
        public void DrawPointCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx)
        {
            // TODO: 目前只能自己抽点数牌，后续需要根据op参数区分抽不同类型的牌
            int selectedCardId = ctx.selectedTargetIds.Count > 0 ? ctx.selectedTargetIds[0] : -1;
            int drawNum = op.value.Evaluate(gameState, ctx, selectedCardId);
            for (int i = 0; i < drawNum; i++)
            {
                StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, () =>
                {
                    int casterId = ctx.caster;
                    if (op.target.participantType == ParticipantType.MyPointCardsOnBoard)
                        casterId = ctx.caster;
                    else if (op.target.participantType == ParticipantType.OpponentPointCardsOnBoard)
                        casterId = ctx.opponent;
                    DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = ctx.caster };
                    gateway.SendCommandServerRpc("DrawPointCard", JsonConvert.SerializeObject(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }

        public void DiscardCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx)
        {
            int selectedCardId = ctx.selectedTargetIds.Count > 0 ? ctx.selectedTargetIds[0] : -1;
            int drawNum = op.value.Evaluate(gameState, ctx, selectedCardId);
            for (int i = 0; i < drawNum; i++)
            {
                StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, () =>
                {
                    int casterId = ctx.caster;
                    if (op.target.participantType == ParticipantType.MyPointCardsOnBoard)
                        casterId = ctx.caster;
                    else if (op.target.participantType == ParticipantType.OpponentPointCardsOnBoard)
                        casterId = ctx.opponent;
                    DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = ctx.caster };
                    gateway.SendCommandServerRpc("DrawPointCard", JsonConvert.SerializeObject(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }

        public IEnumerator ValidateCommand(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, Action onSuccess)
        {
            ValidateSkillCardCommand cmd = new ValidateSkillCardCommand
            {
                playerId = ctx.caster,
                effect = op,
                selectedSourceIds = ctx.selectedSourceIds,
                selectedTargetIds = ctx.selectedTargetIds
            };
            gateway.SendCommandServerRpc("ValidateSkillCard", JsonConvert.SerializeObject(cmd));
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