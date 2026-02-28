using System;

namespace Game.Domain
{
    [Serializable]
    public enum CompareOp
    {
        Greater,
        GreaterEqual,
        Less,
        LessEqual,
        Equal,
    }
}