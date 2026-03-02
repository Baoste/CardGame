using System;
using UnityEngine;

namespace Game.Domain
{
    [Serializable]
    public class ValueExpr
    {
        public virtual int Evaluate(GameState state, EffectContext ctx, int target) { return -1; }   
    }

    [Serializable]
    // 无值
    public class NoneValue : ValueExpr
    {
        public override int Evaluate(GameState state, EffectContext ctx, int target)
            => -1;
    }

    [Serializable]
    // 常量
    public class ConstValue : ValueExpr
    {
        public int value;
        public override int Evaluate(GameState state, EffectContext ctx, int target)
            => value;
    }

    [Serializable]
    // 读取变量
    public class VariableValue : ValueExpr
    {
        public ValueSource source;
        public override int Evaluate(GameState state, EffectContext ctx, int target)
        {
            switch (source)
            {
                case ValueSource.CasterSkillCardsCount:
                    return state.players[ctx.caster].SkillCardsInHand.GetCount();

                case ValueSource.CasterPointCardsCount:
                    return state.players[ctx.caster].PointCardsOnBoard.GetCount();

                case ValueSource.TargetPoints:
                    return CardDatabase.Get(target).point;
            }
            return 0;
        }
    }

    [Serializable]
    // 二元运算
    public class BinaryValue : ValueExpr
    {
        [SerializeReference] public ValueExpr left;
        [SerializeReference] public ValueExpr right;
        public BinaryOp op;

        public override int Evaluate(GameState state, EffectContext ctx, int target)
        {
            int l = left.Evaluate(state, ctx, target);
            int r = right.Evaluate(state, ctx, target);

            return op switch
            {
                BinaryOp.Add => l + r,
                BinaryOp.Sub => l - r,
                BinaryOp.Mul => l * r,
                BinaryOp.Div => r != 0 ? l / r : 0,
                _ => 0
            };
        }
    }
}