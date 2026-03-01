using System;
using UnityEngine;

namespace Game.Domain
{
    [Serializable]
    public class ConditionExpr
    {
        public virtual bool Evaluate(GameState state, EffectContext ctx, int target) { return false; }
    }

    [Serializable]
    public class AllCondition : ConditionExpr
    {
        public override bool Evaluate(GameState state, EffectContext ctx, int target)
        {
            return true;
        }
    }

    // 比较条件
    [Serializable]
    public class CompareCondition : ConditionExpr
    {
        [SerializeReference] public ValueExpr left;
        [SerializeReference] public ValueExpr right;
        public CompareOp op;

        public override bool Evaluate(GameState state, EffectContext ctx, int target)
        {
            int l = left.Evaluate(state, ctx, target);
            int r = right.Evaluate(state, ctx, target);

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
        [SerializeReference] public ConditionExpr a, b;
        public override bool Evaluate(GameState state, EffectContext ctx, int target)
            => a.Evaluate(state, ctx, target) && b.Evaluate(state, ctx, target);
    }
}