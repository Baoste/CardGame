using System;

namespace Game.Domain
{
    public enum CardState
    {
        None,
        Hole,
        Hidden,
        Locked,
    }

    public class PointCard : Card
    {
    }
}