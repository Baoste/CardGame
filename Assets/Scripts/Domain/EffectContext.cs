using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Domain
{
    [Serializable]
    public class EffectContext
    {
        public List<int> selectedSourceIds = new List<int>();       // 已选的来源ID列表，可能是技能牌或点数牌的 InstanceID
        public List<int> selectedTargetIds = new List<int>();       // 已选的目标ID列表，可能是技能牌或点数牌的 InstanceID
        public List<int> candidateSourceIds = new List<int>();      // 可选的来源ID列表，可能是技能牌或点数牌的 InstanceID
        public List<int> candidateTargetIds = new List<int>();      // 可选的目标ID列表，可能是技能牌或点数牌的 InstanceID
        public List<int> tmpSelectedIds = new List<int>();          // 临时选项列表，用于临时储存 selectedSourceIds 或 selectedTargetIds
        public int caster;      // 施法者玩家ID
        public int opponent;    // 对手玩家ID
    }

    public static class ClientEffectContext
    {
        public static bool ChooseDone = false;          // 是否已经选好目标了
        public static bool GetServerCtxDone = false;    // 是否已经拿到 EffectContext 的数据了
        public static bool IsValidateDone = false;      // 是否已经完成了合法性验证了
        public static bool IsCommandValid = false;      // 上一个命令是否合法（比如玩家选的目标是否合法）
        public static bool IsExecuteDone = false;       // 是否正在执行技能卡效果（从玩家选好目标到服务器验证通过再到客户端执行完技能卡效果，这段时间都算在内）
        public static EffectContext Instance;
    }
}