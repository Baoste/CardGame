using System;
using System.Collections.Generic;

namespace Game.Domain
{
    [Serializable]
    public class SkillCard : Card
    {
        public List<EffectOp> effects; // 核心：效果序列
    }
}