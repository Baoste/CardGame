using System;

namespace Game.Domain
{
    public class ValueExpr
    {
        public virtual int Evaluate(GameState state, EffectContext ctx) { return -1; }   
    }

    // 无值
    public class NoneValue : ValueExpr
    {
        public override int Evaluate(GameState state, EffectContext ctx)
            => -1;
    }

    // 常量
    public class ConstValue : ValueExpr
    {
        public int value;
        public override int Evaluate(GameState state, EffectContext ctx)
            => value;
    }

    // 读取变量
    public class VariableValue : ValueExpr
    {
        public ValueSource source;
        public override int Evaluate(GameState state, EffectContext ctx)
        {
            switch (source)
            {
                case ValueSource.CardPointInPool:
                    return -1;

                case ValueSource.CasterSkillCardsCount:
                    return state.players[state.CurrentPlayerId].skillCardsInHand.GetCount();

                case ValueSource.CasterPointCardsCount:
                    return state.players[state.CurrentPlayerId].pointCardsOnBoard.GetCount();

                case ValueSource.SourceSpecSelectedPointsSum:
                {
                    int sum = 0;
                    foreach (int id in ctx.selectedSourceIds)
                    {
                        if (state.instancePointMap.ContainsKey(id))
                            sum += state.instancePointMap[id];
                    }
                    return sum;
                }

                case ValueSource.TargetSpecSelectedPointsSum:
                {
                    int sum = 0;
                    foreach (int id in ctx.selectedTargetIds)
                    {
                        if (state.instancePointMap.ContainsKey(id))
                            sum += state.instancePointMap[id];
                    }
                    return sum;
                }

                case ValueSource.ResolvedCardsPointsSum:
                {
                    return state.SumPointOnCardsToResolve();
                }
            }
            return -1;
        }
    }

    // 二元运算
    public class BinaryValue : ValueExpr
    {
        public ValueExpr left;
        public ValueExpr right;
        public BinaryOp op;

        public override int Evaluate(GameState state, EffectContext ctx)
        {
            int l = left.Evaluate(state, ctx);
            int r = right.Evaluate(state, ctx);

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