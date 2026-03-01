using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain
{
    [Serializable]
    public static class TargetResolver
    {
        public static List<int> DetermineCandidates(TargetSpec spec, GameState state, EffectContext ctx)
        {
            List<int> pool = new List<int>();

            // TargetType 决定从哪来
            if ((spec.targetType & TargetType.MySkillCardsInHand) != 0)
                pool.AddRange(new List<int>(state.players[ctx.caster].SkillCardsInHand));
            if ((spec.targetType & TargetType.OpponentSkillCardsInHand) != 0)
                pool.AddRange(new List<int>(state.players[ctx.opponent].SkillCardsInHand));
            if ((spec.targetType & TargetType.MyPointCardsOnBoard) != 0)
                pool.AddRange(new List<int>(state.players[ctx.caster].PointCardsOnBoard));
            if ((spec.targetType & TargetType.OpponentPointCardsOnBoard) != 0)
                pool.AddRange(new List<int>(state.players[ctx.opponent].PointCardsOnBoard));
            if ((spec.targetType & TargetType.SkillCardsInDeck) != 0)
                pool.AddRange(new List<int>(state.skillCardsDeck.instanceIdsInDeck));
            if ((spec.targetType & TargetType.PointCardsInDeck) != 0)
                pool.AddRange(new List<int>(state.pointCardsDeck.instanceIdsInDeck));

            // filter 过滤
            pool = pool.FindAll(c => spec.filter.Evaluate(state, ctx, c));

            return pool;
        }
        public static bool ValidateTarget(TargetSpec spec, GameState state, EffectContext ctx, out List<int> target)
        {
            target = new List<int>();

            if (ctx.selectedCards.Count > spec.maxPick.Evaluate(state, ctx, -1))
                return false;

            List<int> pool = DetermineCandidates(spec, state, ctx);

            // TargetSelectionMode 决定怎么选
            switch (spec.targetSelectionMode)
            {
                case TargetSelectionMode.All:
                    target = pool;
                    break;
                case TargetSelectionMode.Random:
                    StaticFunction.Shuffle(pool, state.rng);
                    int cid = ctx.selectedCards[0];
                    for (int i = 0; i < spec.maxTargetCount.Evaluate(state, ctx, cid); i++)
                    {
                        if (pool.Count > i)
                        {
                            target.Add(pool[i]);
                        }
                    }
                    break;
                case TargetSelectionMode.Choose:
                    if (!ctx.selectedCards.All(x => pool.Contains(x)))
                        return false;
                    target = ctx.selectedCards;
                    break;
                case TargetSelectionMode.First:
                    if (pool.Count > 0)
                        target.Add(pool[0]);
                    break;
                case TargetSelectionMode.Last:
                    if (pool.Count > 0)
                        target.Add(pool[pool.Count - 1]);
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