namespace Game.Domain
{
    public enum EffectType
    {
        DrawCards,          // 抽牌
        ModifyCardPoints,   // 改变目标牌点数
        MoveCards,          // 移动目标牌
    }

    public class EffectOp
    {
        public EffectType type;
        public TargetSpec target;
        public ValueExpr value;
    }
}