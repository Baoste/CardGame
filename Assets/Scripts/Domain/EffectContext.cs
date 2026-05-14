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

        // 用于执行效果时的操作栈，存储当前正在执行的 EffectOp
        public Stack<EffectOpExecutionContext> opStack = new Stack<EffectOpExecutionContext>();   

        // 用于标识抽点数牌次数
        public int drawPointCardCount = 0;

        public void ClearContext()
        {
            selectedSourceIds.Clear();
            selectedTargetIds.Clear();
            candidateSourceIds.Clear();
            candidateTargetIds.Clear();
            caster = -1;
            opponent = -1;
            drawPointCardCount = 0;
        }
    }

    public class EffectOpExecutionContext
    {
        public EffectOp effectOp;
        public List<int> candidateSourceIds;
        public List<int> candidateTargetIds;
    }

    public static class ClientEffectContext
    {
        public static bool ChooseDone = false;          // 是否已经选好目标了
        public static bool GetServerCtxDone = false;    // 是否已经拿到 EffectContext 的数据了
        public static bool JudgeResult = true;         // 判断结果

        public static bool isExecutingSkillCard = false;
        public static bool isDrawingPointCard = false;
        public static bool isDrawingSkillCard = false;

        public static EffectContext Instance = new EffectContext();
    }
}