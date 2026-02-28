using System;
using System.Collections.Generic;

namespace Game.Domain
{
    [Serializable]
    public static class TargetResolver
    {
        public static List<Card> ResolveTarget(TargetSpec spec, GameState state, EffectContext ctx)
        {
            // 候选池 pool：TargetType 决定从哪来
            List<Card> pool = new List<Card>();
            //if ((spec.targetType & TargetType.CardsInHand) != 0)
            //    pool.AddRange(new List<Card>(state.Players[ctx.caster].Hand));
            //if ((spec.targetType & TargetType.CardsOnBoard) != 0)
            //    pool.AddRange(new List<Card>(state.Players[ctx.caster].Board));
            //if ((spec.targetType & TargetType.SkillCardsInDeck) != 0)
            //    pool.AddRange(List<Card>(state.SkillCardsDeck));
            //if ((spec.targetType & TargetType.PointCardsInDeck) != 0)
            //    pool.AddRange((List<Card>(state.PointCardsDeck));

            //// 过滤（如果你有 ConditionExpr）
            //if (spec.condition != null)
            //    pool = pool.FindAll(c => spec.condition.Evaluate(state, ctx, c));

            //// 随机（如果是随机目标）
            //if (spec.targetSelectionMode == TargetSelectionMode.Random)
            //{
            //    if (pool.Count == 0) return new List<Card>();
            //    int i = state.rng.Next(pool.Count);
            //    pool = new List<Card> { pool[i] };
            //}

            //// 排序 + TopK（如果你有 sortKey）
            //// if (spec.sortKey != null)
            //// {
            ////     pool.Sort((a, b) =>
            ////     {
            ////         int ka = spec.sortKey.Evaluate(state, ctx, a);
            ////         int kb = spec.sortKey.Evaluate(state, ctx, b);
            ////         return spec.sortDescending ? kb.CompareTo(ka) : ka.CompareTo(kb);
            ////     });
            //// }

            int take = spec.count <= 0 ? pool.Count : System.Math.Min(spec.count, pool.Count);
            return pool.GetRange(0, take);
        }
    }
}