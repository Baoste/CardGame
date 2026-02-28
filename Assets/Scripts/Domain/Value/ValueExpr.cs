namespace Game.Domain
{
    public abstract class ValueExpr
    {
        public abstract int Evaluate(GameState state, EffectContext ctx, Card target);
    }

    // 常量
    public class ConstValue : ValueExpr
    {
        public int value;
        public override int Evaluate(GameState state, EffectContext ctx, Card target)
            => value;
    }

    // 读取变量
    public class VariableValue : ValueExpr
    {
        public ValueSource source;
        public override int Evaluate(GameState state, EffectContext ctx, Card target)
        {
            switch (source)
            {
                case ValueSource.CasterHandCount:
                    return state.players[ctx.caster].handCount;

                case ValueSource.TargetPoints:
                    return target.point;
            }
            return 0;
        }
    }

    // 二元运算
    public class BinaryValue : ValueExpr
    {
        public ValueExpr left;
        public ValueExpr right;
        public BinaryOp op;

        public override int Evaluate(GameState state, EffectContext ctx, Card target)
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