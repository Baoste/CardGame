namespace Game.Domain
{
    // TODO: 需要改成ZoneType，表示牌所在的区域，例如手牌、场上、牌堆等
    public enum AnimationType
    {
        MoveToFallPosition,
        ReturnToHand,
        MoveToExecutePosition
    }
}