using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Domain
{
    public class EffectContext
    {
        public List<int> selectedSourceIds = new List<int>();       // 已选的来源ID列表，可能是技能牌或点数牌的 InstanceID
        public List<int> selectedTargetIds = new List<int>();       // 已选的目标ID列表，可能是技能牌或点数牌的 InstanceID
        public List<int> candidateSourceIds = new List<int>();      // 可选的来源ID列表，可能是技能牌或点数牌的 InstanceID
        public List<int> candidateTargetIds = new List<int>();      // 可选的目标ID列表，可能是技能牌或点数牌的 InstanceID
        // 技能卡效果需要用到这两个字段来区分玩家
        public int caster;      // 施法者玩家ID
        public int opponent;    // 对手玩家ID

        public void ClearContext()
        {
            selectedSourceIds.Clear();
            selectedTargetIds.Clear();
            candidateSourceIds.Clear();
            candidateTargetIds.Clear();
            caster = -1;
            opponent = -1;
        }
    }

    public static class ClientEffectContext
    {
        public static bool ChooseDone = false;          // 是否已经选好目标了
        public static bool GetServerCtxDone = false;    // 是否已经拿到 EffectContext 的数据了
        
        // 是否已经完成了合法性验证了
        public static bool _isValidateDone = false;      
        public static bool IsValidateDone
        {
            get
            {
                bool value = _isValidateDone;
                _isValidateDone = false;   // 读取后自动重置
                return value;
            }
            set
            {
                _isValidateDone = value;
            }
        }

        // 上一个命令是否合法（比如玩家选的目标是否合法）
        private static bool _isCommandValid = false;
        public static bool IsCommandValid
        {
            get
            {
                bool value = _isCommandValid;
                _isCommandValid = false;   // 读取后自动重置
                return value;
            }
            set
            {
                _isCommandValid = value;
            }
        }

        public static bool isExecutingSkillCard = false;
        // 是否正在执行技能卡效果（从玩家选好目标到服务器验证通过再到客户端执行完技能卡效果，这段时间都算在内）
        private static bool _isExecuteDone = false;
        public static bool IsExecuteDone
        {
            get
            {
                bool value = _isExecuteDone;
                _isExecuteDone = false;   // 读取后自动重置
                return value;
            }
            set
            {
                _isExecuteDone = value;
            }
        }
        public static EffectContext Instance = new EffectContext();
    }
}