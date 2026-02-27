using System.Collections.Generic;

namespace Game.Domain
{
    public class EffectContext
    {
        public List<Card> candidateCards = new();   // 候选池（展示给玩家用）
        public List<Card> selectedCards  = new();   // 玩家最终选中的目标（结算用）
        public int caster;
    }
}