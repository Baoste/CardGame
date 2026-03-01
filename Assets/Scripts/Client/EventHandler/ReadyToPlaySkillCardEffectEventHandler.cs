using Game.Domain;
using System.Collections.Generic;
using UnityEngine;

public sealed class ReadyToPlaySkillCardEffectEventHandler : IEventProcess, IEventHandler
{
    private ReadyToPlaySkillCardEffectEvent payload;

    public bool Handle(NetEvent ev)
    {
        payload = JsonUtility.FromJson<ReadyToPlaySkillCardEffectEvent>(ev.jsonData); // need change
        ProcessQueueManager.Instance.Enqueue(Process);

        // TODO
        // START
        ClientEffectContext.Instance.candidateCards = payload.candidateIds;
        List<int> targetIds = payload.candidateIds;

        string context = "";
        if (targetIds.Count > 0)
        {
            context = $"targets:{string.Join(",", targetIds)}";
        }
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }
    public void Process()
    {
        // TODO: 这里之后要写一个专门的函数来处理这个事件，暂时先写个测试函数
        // 这里的函数是为了打出技能卡之后，返回了一个候选目标列表，玩家需要选择一个目标来执行技能卡的效果
        // 所以这个函数的作用就是把候选目标列表展示出来，让玩家选择一个目标
        ProcessDispatcher.Process("PlaySkillCardTest", new object[] { payload.candidateIds });
    }
}