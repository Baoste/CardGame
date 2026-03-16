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
        public void ExecuteOp(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> selectedSourceIds, List<int> selectedTargetIds)
        {
            switch (op.type)
            {
                case EffectType.DrawPoint:
                    StartCoroutine(DrawPointCards(op, gateway, gameState, ctx));
                    break;

                case EffectType.DrawSkill:
                    StartCoroutine(DrawSkillCards(op, gateway, gameState, ctx));
                    break;

                case EffectType.DrawPointToResolve:
                    ClientEffectContext.IsExecuteDone = true;  // 这个效果不需要客户端执行，直接告诉流程继续往下走就行了
                    break;

                case EffectType.Discard:
                    StartCoroutine(DiscardCards(op, gateway, gameState, ctx, selectedTargetIds));
                    break;

                case EffectType.ModifyPoint:
                    StartCoroutine(ModifyPoint(op, gateway, gameState, ctx, selectedTargetIds));
                    break;

                case EffectType.Move:
                    StartCoroutine(MoveCards(op, gateway, gameState, ctx, selectedSourceIds, selectedTargetIds));
                    break;

                case EffectType.Judge:
                    ClientEffectContext.IsExecuteDone = true;  // 这个效果不需要客户端执行，直接告诉流程继续往下走就行了
                    break;
            }
        }

        public IEnumerator DrawPointCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx)
        {
            // TODO: 目前只能自己抽点数牌，后续需要根据op参数区分抽不同类型的牌
            int drawNum = op.value.Evaluate(gameState, ctx);

            int casterId = ctx.caster;
            int opponentId = ctx.opponent;
            ParticipantType participantType = op.target.participantType;

            for (int i = 0; i < drawNum; i++)
            {
                yield return StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, new List<int>(), new List<int>(), () =>
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

        private IEnumerator DrawSkillCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx)
        {
            int drawNum = op.value.Evaluate(gameState, ctx);

            int casterId = ctx.caster;
            int opponentId = ctx.opponent;
            ParticipantType participantType = op.target.participantType;

            for (int i = 0; i < drawNum; i++)
            {
                yield return StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, new List<int>(), new List<int>(), () =>
                {
                    int playerId = casterId;
                    // 根据op参数区分抽到不同的玩家手里
                    if (participantType == ParticipantType.OpponentSkillCardsInHand)
                        playerId = opponentId;
                    DrawSkillCardCommand cmd = new DrawSkillCardCommand { playerId = playerId };
                    gateway.SendCommandServerRpc("DrawSkillCard", JsonConvert.SerializeObject(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }

        public IEnumerator DiscardCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> selectedTargetIds)
        {
            int count = selectedTargetIds.Count;
            List<int> _selectedTargetIds = selectedTargetIds;
            for (int i = 0; i < count; i++)
            {
                int idx = i;    // 防止闭包捕获
                yield return StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, new List<int>(), selectedTargetIds, () =>
                {
                    DiscardCardCommand cmd = new DiscardCardCommand { playerId = ctx.caster, instanceId = _selectedTargetIds[idx] };
                    gateway.SendCommandServerRpc("DiscardCard", JsonConvert.SerializeObject(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }

        public IEnumerator ModifyPoint(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> selectedTargetIds)
        {
            int count = selectedTargetIds.Count;
            List<int> _selectedTargetIds = selectedTargetIds;
            int value = op.value.Evaluate(gameState, ctx);
            for (int i = 0; i < count; i++)
            {
                int idx = i;    // 防止闭包捕获
                yield return StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, new List<int>(), selectedTargetIds, () =>
                {
                    ModifyPointCommand cmd = new ModifyPointCommand { playerId = ctx.caster, instanceId = _selectedTargetIds[idx], pointChange = value };
                    gateway.SendCommandServerRpc("ModifyPoint", JsonConvert.SerializeObject(cmd));
                }));
            }
            ClientEffectContext.IsExecuteDone = true;
        }

        public IEnumerator MoveCards(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> selectedSourceIds, List<int> selectedTargetIds)
        {
            int count = selectedSourceIds.Count;
            List<int> _selectedSourceIds = selectedSourceIds;

            ParticipantType selectZone;
            if (selectedTargetIds.Count == 0)
            {
                selectZone = op.target.participantType;
            }
            else
            {
                selectZone = (ParticipantType)selectedTargetIds[0];
            }
            for (int i = 0; i < count; i++)
            {
                int idx = i;    // 防止闭包捕获
                yield return StartCoroutine(ValidateCommand(op, gateway, gameState, ctx, selectedSourceIds, selectedTargetIds, () =>
                {
                    MoveCardCommand cmd = new MoveCardCommand { playerId = ctx.caster, instanceId = _selectedSourceIds[idx], toZone=selectZone };
                    gateway.SendCommandServerRpc("MoveCard", JsonConvert.SerializeObject(cmd));
                }));
            }

            if (op.source.participantType == ParticipantType.CardsToResolve)
            {
                ClearCardsToResolveCommand cmd2 = new ClearCardsToResolveCommand { playerId = ctx.caster };
                gateway.SendCommandServerRpc("ClearCardsToResolve", JsonConvert.SerializeObject(cmd2), ClientGameState.playerSlot);
            }

            ClientEffectContext.IsExecuteDone = true;
        }

        public IEnumerator ValidateCommand(EffectOp op, MatchGateway gateway, GameState gameState, EffectContext ctx, List<int> selectedSourceIds, List<int> selectedTargetIds, Action onSuccess)
        {
            ValidateParticipantsCommand cmd = new ValidateParticipantsCommand
            {
                playerId = ctx.caster,
                effect = op,
                selectedSourceIds = selectedSourceIds,
                selectedTargetIds = selectedTargetIds
            };
            gateway.SendCommandServerRpc("ValidateParticipants", JsonConvert.SerializeObject(cmd), ClientGameState.playerSlot);
            yield return new WaitUntil(() => ClientEffectContext.IsValidateDone);
            ClientEffectContext.IsValidateDone = false;

            if (!ClientEffectContext.IsCommandValid)
            {
                Debug.Log($"[Client] Validate Cmd {op.type} failed");
                yield break;
            }
            onSuccess.Invoke();
        }
    }
}