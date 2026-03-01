using System;
using System.Collections.Generic;

namespace Game.Domain
{
    [Serializable]
    public enum CardType
    {
        Point,
        Skill
    }

    [Serializable]
    public class Card
    {
        public int id;
        public string name;
        public string description;
        public int point;
        public CardType type;

        public List<EffectOp> effects; // 核心：效果序列
    }
}