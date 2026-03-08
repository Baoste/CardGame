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
            int drawNum = op.value.Evaluate(gameState, ctx);

            int casterId = ctx.caster;
            int opponentId = ctx.opponent;
            ParticipantType participantType = op.target.participantType;

            for (int i = 0; i < drawNum; i++)
            {
                StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, () =>
                {
                    int playerId = casterId;
                    if (participantType == ParticipantType.OpponentPointCardsOnBoard)
                        playerId = opponentId;
                    DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = playerId };
                    gateway.SendCommandServerRpc("DrawPointCard", JsonConvert.SerializeObject(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }

        public void DrawSkillCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx)
        {
            // TODO: 目前只能自己抽点数牌，后续需要根据op参数区分抽不同类型的牌
            int drawNum = op.value.Evaluate(gameState, ctx);

            int casterId = ctx.caster;
            int opponentId = ctx.opponent;
            ParticipantType participantType = op.target.participantType;

            for (int i = 0; i < drawNum; i++)
            {
                StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, () =>
                {
                    int playerId = casterId;
                    if (participantType == ParticipantType.OpponentSkillCardsInHand)
                        playerId = opponentId;
                    DrawSkillCardCommand cmd = new DrawSkillCardCommand { playerId = playerId };
                    gateway.SendCommandServerRpc("DrawSkillCard", JsonConvert.SerializeObject(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }

        public void DiscardCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> selectedTargetIds)
        {
            int count = selectedTargetIds.Count;
            List<int> _selectedTargetIds = selectedTargetIds;
            for (int i = 0; i < count; i++)
            {
                int idx = i;    // 防止闭包捕获
                StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, () =>
                {
                    DiscardCardCommand cmd = new DiscardCardCommand { playerId = ctx.caster, instanceId = _selectedTargetIds[idx] };
                    gateway.SendCommandServerRpc("DiscardCard", JsonConvert.SerializeObject(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }

        public void ModifyPoint(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> selectedTargetIds)
        {
            int count = selectedTargetIds.Count;
            List<int> _selectedTargetIds = selectedTargetIds;
            int value = op.value.Evaluate(gameState, ctx);
            for (int i = 0; i < count; i++)
            {
                int idx = i;    // 防止闭包捕获
                StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, () =>
                {
                    ModifyPointCommand cmd = new ModifyPointCommand { playerId = ctx.caster, instanceId = _selectedTargetIds[idx], pointChange = value };
                    gateway.SendCommandServerRpc("ModifyPoint", JsonConvert.SerializeObject(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }

        public void MoveCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> selectedSourceIds)
        {
            int count = selectedSourceIds.Count;
            List<int> _selectedSourceIds = selectedSourceIds;
            for (int i = 0; i < count; i++)
            {
                int idx = i;    // 防止闭包捕获
                StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, () =>
                {
                    MoveCardCommand cmd = new MoveCardCommand { playerId = ctx.caster, instanceId = _selectedSourceIds[idx] };
                    gateway.SendCommandServerRpc("MoveCard", JsonConvert.SerializeObject(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }

        public IEnumerator ValidateCommand(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, Action onSuccess)
        {
            ValidateParticipantsCommand cmd = new ValidateParticipantsCommand
            {
                playerId = ctx.caster,
                effect = op,
                selectedSourceIds = ctx.selectedSourceIds,
                selectedTargetIds = ctx.selectedTargetIds
            };
            gateway.SendCommandServerRpc("ValidateParticipants", JsonConvert.SerializeObject(cmd), ClientGameState.playerSlot);
            yield return new WaitUntil(() => ClientEffectContext.IsValidateDone);
            ClientEffectContext.IsValidateDone = false;

            if (!ClientEffectContext.IsCommandValid)
            {
                Debug.Log($"[Client] Validate Cmd failed");
                yield break;
            }
            onSuccess.Invoke();
        }
    }
}