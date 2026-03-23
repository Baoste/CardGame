using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain
{
    public static class ParticipantResolver
    {
        public static List<int> DetermineCandidates(ParticipantSpec spec, GameState state, EffectContext ctx, out bool isParticipantZone)
        {
            List<int> pool = new List<int>();
            isParticipantZone = false;

            // ParticipantType 决定从哪来
            if ((spec.participantType & ParticipantType.MySkillCardsInHand) != 0)
                pool.AddRange(new List<int>(state.players[ctx.caster].skillCardsInHand.instanceIds));
            if ((spec.participantType & ParticipantType.OpponentSkillCardsInHand) != 0)
                pool.AddRange(new List<int>(state.players[ctx.opponent].skillCardsInHand.instanceIds));
            if ((spec.participantType & ParticipantType.MyPointCardsOnBoard) != 0)
                pool.AddRange(new List<int>(state.players[ctx.caster].pointCardsOnBoard.instanceIds));
            if ((spec.participantType & ParticipantType.OpponentPointCardsOnBoard) != 0)
                pool.AddRange(new List<int>(state.players[ctx.opponent].pointCardsOnBoard.instanceIds));
            if ((spec.participantType & ParticipantType.SkillCardsInDeck) != 0)
                pool.AddRange(new List<int>(state.skillCardsDeck.instanceIds));
            if ((spec.participantType & ParticipantType.PointCardsInDeck) != 0)
                pool.AddRange(new List<int>(state.pointCardsDeck.instanceIds));
            if ((spec.participantType & ParticipantType.CardsToResolve) != 0)
                pool.AddRange(new List<int>(state.cardsToResolve.instanceIds));

            // filter 过滤
            pool = pool.FindAll(c => spec.filter.Evaluate(state, ctx, state.instancePointMap[c]));

            // 如果是Zone区域类别
            if ((spec.participantType & ParticipantType.MyBoardZone) != 0)
            {
                pool.Add((int)ParticipantType.MyBoardZone);
                isParticipantZone = true;
            }
            if ((spec.participantType & ParticipantType.OppentBoardZone) != 0)
            {
                pool.Add((int)ParticipantType.OppentBoardZone);
                isParticipantZone = true;
            }

            return pool;
        }
    }
}