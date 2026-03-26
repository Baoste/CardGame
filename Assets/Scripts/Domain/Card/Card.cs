using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public enum CardType
    {
        Point,
        Skill,
    }

    public class Card
    {
        public int id;
        public string name;
        public string description;
        public int point;

        public CardType type;
        public int count;       // 生成牌堆中的初始数量
        public List<EffectOp> effects; // 核心：效果序列，要改成一个二叉树
    }
}