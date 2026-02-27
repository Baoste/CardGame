//namespace Game.Domain
//{
//    // 生成候选池（只做一次），一定要把候选池“锁住”，不要在玩家点选期间反复调用 ResolveCardTargets
//    public static void BuildCandidates(EffectOp op, GameState state, EffectContext ctx)
//    {
//        ctx.candidateCards.Clear();
//        ctx.candidateCards.AddRange(ResolveCardTargets(op.target, state, ctx));
//    }

//    // 玩家选择
//    public static bool ToggleSelect(Card clicked, EffectOp op, EffectContext ctx)
//    {
//        int maxPick = op.target.maxPick;
//        if (!ctx.candidateCards.Contains(clicked))
//            return false;

//        if (ctx.selectedCards.Contains(clicked))
//        {
//            ctx.selectedCards.Remove(clicked);
//            return true;
//        }

//        if (ctx.selectedCards.Count >= maxPick)
//            return false;

//        ctx.selectedCards.Add(clicked);
//        return true;
//    }

//    public static void DrawCards(EffectOp op, GameState state, EffectContext ctx)
//    {
//        int drawNum = op.value.Evaluate(state, ctx, null);
//        for (int i = 0; i < drawNum; i++)
//        {
//            // TODO: send draw event
//        }
//    }

//    public static void ModifyCardPoints(EffectOp op, GameState state, EffectContext ctx)
//    {
//        foreach (var card in ctx.selectedCards)
//        {
//            // TODO: send modify point event
//            // card.Points += op.value.Evaluate(state, ctx, card);
//        }
//    }
//}