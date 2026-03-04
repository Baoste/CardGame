using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain
{
    public static class ParticipantResolver
    {
        public static bool ValidateCard(ParticipantSpec spec, GameState state, EffectContext ctx)
        {
            if (spec.participantType == ParticipantType.None)
                return true;
            List<int> pool = DetermineCandidates(spec, state, ctx);
            switch (spec.participantSelectionMode)
            {
                case ParticipantSelectionMode.None:
                    return true;
                case ParticipantSelectionMode.All:
                    if (pool.Count == 0)
                        return false;
                    break;
                case ParticipantSelectionMode.Random:
                    if (pool.Count == 0)
                        return false;
                    break;
                case ParticipantSelectionMode.Choose:
                    if (pool.Count == 0)
                        return false;
                    break;
                case ParticipantSelectionMode.First:
                    if (pool.Count == 0)
                        return false;
                    break;
                case ParticipantSelectionMode.Last:
                    if (pool.Count == 0)
                        return false;
                    break;
            }
            return true;
        }

        public static List<int> DetermineCandidates(ParticipantSpec spec, GameState state, EffectContext ctx)
        {
            List<int> pool = new List<int>();

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

            // filter 过滤
            pool = pool.FindAll(c => spec.filter.Evaluate(state, ctx, c));

            return pool;
        }
        public static bool ValidateParticipant(ParticipantSpec spec, GameState state, EffectContext ctx, out List<int> participantIds)
        {
            participantIds = new List<int>();

            if (spec.participantType == ParticipantType.None)
                return ctx.tmpSelectedIds.Count == 0;

            List<int> pool = DetermineCandidates(spec, state, ctx);

            // ParticipantSelectionMode 决定怎么选
            switch (spec.participantSelectionMode)
            {
                case ParticipantSelectionMode.None:
                    return true;
                case ParticipantSelectionMode.All:
                    if (pool.Count == 0)
                        return false;
                    participantIds = pool;
                    break;
                case ParticipantSelectionMode.Random:
                    StaticFunction.Shuffle(pool, state.rng);
                    int cid = ctx.tmpSelectedIds.Count > 0 ? ctx.tmpSelectedIds[0] : -1;
                    for (int i = 0; i < spec.maxCandidateCountWhenRandom.Evaluate(state, ctx, cid); i++)
                    {
                        if (pool.Count > i)
                        {
                            participantIds.Add(pool[i]);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;
                case ParticipantSelectionMode.Choose:
                    if (!ctx.tmpSelectedIds.All(x => pool.Contains(x)))
                        return false;
                    participantIds = ctx.tmpSelectedIds;
                    break;
                case ParticipantSelectionMode.First:
                    if (pool.Count > 0)
                        participantIds.Add(pool[0]);
                    else
                        return false;
                    break;
                case ParticipantSelectionMode.Last:
                    if (pool.Count > 0)
                        participantIds.Add(pool[pool.Count - 1]);
                    else
                        return false;
                    break;
            }

            return true;

            // // 排序 + TopK（如果你有 sortKey）
            // if (spec.sortKey != null)
            // {
            //     pool.Sort((a, b) =>
            //     {
            //         int ka = spec.sortKey.Evaluate(state, ctx, a);
            //         int kb = spec.sortKey.Evaluate(state, ctx, b);
            //         return spec.sortDescending ? kb.CompareTo(ka) : ka.CompareTo(kb);
            //     });
            // }
        }
    }
}