//namespace Game.Domain
//{
//    public bool Validate(GameState state, ICommand cmd, out string reason)
//    {
//        // TODO
//        return true;
//    }

//    public void Resolve(GameState state, ICommand cmd, List<GameEvent> events)
//    {
//        cmd.Resolve();
//    }

//    static void ResolveDraw(GameState state, DrawCardCommand cmd, List<GameEvent> events)
//    {
//    }

//    static void ResolvePlay(GameState state, PlayCardCommand cmd, List<GameEvent> events)
//    {
//        Card card = CardDatabase.Get(cmd.CardId);
//        ctx.selectedCards = cmd.targetIds;
//        ExecuteCard(card, state, ctx); // 里面要加上events.Add
//        // 广播发生在网络层，不在 Domain
//    }
//}