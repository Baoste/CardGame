using Game.Domain;

public static class DispatcherBootstrap
{
    public static void Init()
    {
        EventDispatcher.Register("JoinOrCreateMatch", new JoinOrCreateMatchEventHandler());
        EventDispatcher.Register("StartMatch", new StartMatchEventHandler());
        EventDispatcher.Register("StartGame", new StartGameEventHandler());
        EventDispatcher.Register("InvalidAction", new InvalidActionEventHandler());
        EventDispatcher.Register("AssignRoles", new AssignRolesEventHandler());
        EventDispatcher.Register("Place1Bet", new Place1BetEventHandler());
        EventDispatcher.Register("ConfirmBet", new ConfirmBetEventHandler());
        EventDispatcher.Register("StartTurn", new StartTurnEventHandler());
        EventDispatcher.Register("GetGameState", new GetGameStateEventHandler());
        EventDispatcher.Register("GetCtx", new GetCtxEventHandler());
        EventDispatcher.Register("Chat", new ChatEventHandler());
        EventDispatcher.Register("WaitForPlayer2Choose", new WaitForPlayer2ChooseEventHandler());
        EventDispatcher.Register("PlayAnimation", new PlayAnimationEventHandler());
        EventDispatcher.Register("ValidateActionPoint", new ValidateActionPointEventHandler());
        EventDispatcher.Register("AddActionPoint", new AddActionPointEventHandler());
        EventDispatcher.Register("SpendActionPoint", new SpendActionPointEventHandler());
        EventDispatcher.Register("DrawPointCard", new DrawPointCardEventHandler());
        EventDispatcher.Register("DrawSkillCard", new DrawSkillCardEventHandler());
        EventDispatcher.Register("DrawPointCardToResolve", new DrawPointCardToResolveEventHandler());
        EventDispatcher.Register("ClearCardsToResolve", new ClearCardsToResolveEventHandler());
        EventDispatcher.Register("DiscardCard", new DiscardCardEventHandler());
        EventDispatcher.Register("ModifyPoint", new ModifyPointEventHandler());
        EventDispatcher.Register("MoveCard", new MoveCardEventHandler());
        EventDispatcher.Register("ChangeCardState", new ChangeCardStateEventHandler());
        EventDispatcher.Register("PeekTopCard", new PeekTopCardEventHandler());
        EventDispatcher.Register("RevealCardsAndScore", new RevealCardsAndScoreEventHandler());
        EventDispatcher.Register("EndTurn", new EndTurnEventHandler());

        CommandDispatcher.Register("LeaveMatch", new LeaveMatchCmdHandler());
        CommandDispatcher.Register("JoinOrCreateMatch", new JoinOrCreateMatchCmdHandler());
        CommandDispatcher.Register("StartMatch", new StartMatchCmdHandler());
        CommandDispatcher.Register("StartGame", new StartGameCmdHandler());
        CommandDispatcher.Register("AssignRoles", new AssignRolesCmdHandler());
        CommandDispatcher.Register("Place1Bet", new Place1BetCmdHandler());
        CommandDispatcher.Register("ConfirmBet", new ConfirmBetCmdHandler());
        CommandDispatcher.Register("StartTurn", new StartTurnCmdHandler());
        CommandDispatcher.Register("GetGameState", new GetGameStateCmdHandler());
        CommandDispatcher.Register("GetCtx", new GetCtxCmdHandler());
        CommandDispatcher.Register("Chat", new ChatCmdHandler());
        CommandDispatcher.Register("StartExecuteSkill", new StartExecuteSkillCmdHandler());
        CommandDispatcher.Register("CommitChosenIds", new CommitChosenIdsCmdHandler());
        CommandDispatcher.Register("PlayAnimation", new PlayAnimationCmdHandler());
        CommandDispatcher.Register("ValidateActionPoint", new ValidateActionPointCmdHandler());
        CommandDispatcher.Register("AddActionPoint", new AddActionPointCmdHandler());
        CommandDispatcher.Register("SpendActionPoint", new SpendActionPointCmdHandler());
        CommandDispatcher.Register("DrawPointCard", new DrawPointCardCmdHandler());
        CommandDispatcher.Register("DrawSkillCard", new DrawSkillCardCmdHandler());
        CommandDispatcher.Register("ClearCardsToResolve", new ClearCardsToResolveCmdHandler());
        CommandDispatcher.Register("DiscardCard", new DiscardCardCmdHandler());
        //CommandDispatcher.Register("ModifyPoint", new ModifyPointCmdHandler());
        //CommandDispatcher.Register("MoveCard", new MoveCardCmdHandler());
        //CommandDispatcher.Register("ChangeCardState", new ChangeCardStateCmdHandler());
        //CommandDispatcher.Register("PeekTopCard", new PeekTopCardCmdHandler());
        CommandDispatcher.Register("RevealCardsAndScore", new RevealCardsAndScoreCmdHandler());
        CommandDispatcher.Register("EndTurn", new EndTurnCmdHandler());
    }
}