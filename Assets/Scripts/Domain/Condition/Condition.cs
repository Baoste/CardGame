using System;

namespace Game.Domain
{
    [Serializable]
    public abstract class ConditionExpr
    {
        public abstract bool Evaluate(GameState state, EffectContext ctx, Card card);
    }

    // 比较条件
    [Serializable]
    public class CompareCondition : ConditionExpr
    {
        public ValueExpr left;
        public ValueExpr right;
        public CompareOp op;

        public override bool Evaluate(GameState state, EffectContext ctx, Card card)
        {
            int l = left.Evaluate(state, ctx, card);
            int r = right.Evaluate(state, ctx, card);

            return op switch
            {
                CompareOp.Greater => l > r,
                CompareOp.GreaterEqual => l >= r,
                CompareOp.Less => l < r,
                CompareOp.LessEqual => l <= r,
                CompareOp.Equal => l == r,
                _ => false
            };
        }
    }

    // 逻辑组合
    [Serializable]
    public class AndCondition : ConditionExpr
    {
        public ConditionExpr a, b;
        public override bool Evaluate(GameState state, EffectContext ctx, Card card)
            => a.Evaluate(state, ctx, card) && b.Evaluate(state, ctx, card);
    }
}