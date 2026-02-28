using System;

namespace Game.Domain
{
    [Serializable]
    public class Card
    {
        public int id;
        public string name;
        public string description;
        public int point;
    }
}