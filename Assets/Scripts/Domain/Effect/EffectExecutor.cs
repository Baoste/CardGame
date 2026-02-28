using System;

namespace Game.Domain
{
    [Serializable]
    public static class EffectExecutor
    {
        public static void ExecuteCard(SkillCard card,  GameState state, EffectContext ctx)
        {
            foreach (var op in card.effects)
                ExecuteOp(op, state, ctx);
        }

        private static void ExecuteOp(EffectOp op, GameState state, EffectContext ctx)
        {
            switch (op.type)
            {
                case EffectType.DrawCards:
                    //DrawCards(op, state, ctx);
                    break;

                case EffectType.ModifyCardPoints:
                    //ModifyCardPoints(op, state, ctx);
                    break;
            }
        }
    }
}