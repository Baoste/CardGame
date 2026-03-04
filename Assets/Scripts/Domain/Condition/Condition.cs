using System;

namespace Game.Domain
{
    public class ConditionExpr
    {
        public virtual bool Evaluate(GameState state, EffectContext ctx) { return false; }
    }

    public class NoneCondition : ConditionExpr
    {
        public override bool Evaluate(GameState state, EffectContext ctx)
        {
            return false;
        }
    }

    public class AllCondition : ConditionExpr
    {
        public override bool Evaluate(GameState state, EffectContext ctx)
        {
            return true;
        }
    }

    // 比较条件
    public class CompareCondition : ConditionExpr
    {
        public ValueExpr left;
        public ValueExpr right;
        public CompareOp op;

        public override bool Evaluate(GameState state, EffectContext ctx)
        {
            int l = left.Evaluate(state, ctx);
            int r = right.Evaluate(state, ctx);

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
    public class AndCondition : ConditionExpr
    {
        public ConditionExpr a, b;
        public override bool Evaluate(GameState state, EffectContext ctx)
            => a.Evaluate(state, ctx) && b.Evaluate(state, ctx);
    }
}