using System;

namespace Game.Domain
{
    public class ConditionExpr
    {
        public virtual bool Evaluate(GameState state, EffectContext ctx, int point) { return false; }
    }

    public class NoneCondition : ConditionExpr
    {
        public override bool Evaluate(GameState state, EffectContext ctx, int point)
        {
            return false;
        }
    }

    public class AllCondition : ConditionExpr
    {
        public override bool Evaluate(GameState state, EffectContext ctx, int point)
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

        public override bool Evaluate(GameState state, EffectContext ctx, int point)
        {
            int l = left.Evaluate(state, ctx);
            int r = right.Evaluate(state, ctx);

            if (l == -1)
                l = point;
            if (r == -1)
                r = point;

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

    //单双条件
    public class OddEvenCondition : ConditionExpr
    {
        public ValueExpr left;
        public ValueExpr right;

        public override bool Evaluate(GameState state, EffectContext ctx, int point)
        {
            int l = left.Evaluate(state, ctx);
            int r = right.Evaluate(state, ctx);

            if (l == -1)
                l = point;
            if (r == -1)
                r = point;

            if (l % 2 == r % 2)
                return true;
            else
                return false;
        }
    }

    // 逻辑组合
    public class AndCondition : ConditionExpr
    {
        public ConditionExpr a, b;
        public override bool Evaluate(GameState state, EffectContext ctx, int point)
            => a.Evaluate(state, ctx, point) && b.Evaluate(state, ctx, point);
    }
}