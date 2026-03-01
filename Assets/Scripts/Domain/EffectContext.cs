using System.Collections.Generic;

namespace Game.Domain
{
    public class EffectContext
    {
        public List<int> candidateCards = new List<int>();   // 候选池（展示给玩家用）
        public List<int> selectedCards = new List<int>();    // 玩家最终选中的目标（结算用）
        public int caster;      // 施法者玩家ID
        public int opponent;    // 对手玩家ID
    }
}