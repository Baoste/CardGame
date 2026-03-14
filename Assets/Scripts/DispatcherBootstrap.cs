using Game.Domain;

public static class DispatcherBootstrap
{
    public static void Init()
    {
        EventDispatcher.Register("JoinOrCreateGame", new JoinOrCreateGameEventHandler());
        EventDispatcher.Register("StartGame", new StartGameEventHandler());
        EventDispatcher.Register("StartTurn", new StartTurnEventHandler());
        EventDispatcher.Register("GetGameState", new GetGameStateEventHandler());
        EventDispatcher.Register("GetCtx", new GetCtxEventHandler());
        EventDispatcher.Register("Chat", new ChatEventHandler());
        EventDispatcher.Register("ValidateActionPoint", new ValidateActionPointEventHandler());
        EventDispatcher.Register("SpendActionPoint", new SpendActionPointEventHandler());
        EventDispatcher.Register("DrawPointCard", new DrawPointCardEventHandler());
        EventDispatcher.Register("DrawSkillCard", new DrawSkillCardEventHandler());
        EventDispatcher.Register("DrawPointCardToResolve", new DrawPointCardToResolveEventHandler());
        EventDispatcher.Register("ClearCardsToResolve", new ClearCardsToResolveEventHandler());
        EventDispatcher.Register("DiscardCard", new DiscardCardEventHandler());
        EventDispatcher.Register("ModifyPoint", new ModifyPointEventHandler());
        EventDispatcher.Register("MoveCard", new MoveCardEventHandler());
        EventDispatcher.Register("RevealCardsAndScore", new RevealCardsAndScoreEventHandler());
        EventDispatcher.Register("DetermineParticipants", new DetermineParticipantsEventHandler());
        EventDispatcher.Register("ValidateParticipants", new ValidateParticipantsEventHandler());
        EventDispatcher.Register("EndTurn", new EndTurnEventHandler());

        CommandDispatcher.Register("LeaveGame", new LeaveGameCmdHandler());
        CommandDispatcher.Register("JoinOrCreateGame", new JoinOrCreateGameCmdHandler());
        CommandDispatcher.Register("StartGame", new StartGameCmdHandler());
        CommandDispatcher.Register("StartTurn", new StartTurnCmdHandler());
        CommandDispatcher.Register("GetGameState", new GetGameStateCmdHandler());
        CommandDispatcher.Register("GetCtx", new GetCtxCmdHandler());
        CommandDispatcher.Register("Chat", new ChatCmdHandler());
        CommandDispatcher.Register("ValidateActionPoint", new ValidateActionPointCmdHandler());
        CommandDispatcher.Register("SpendActionPoint", new SpendActionPointCmdHandler());
        CommandDispatcher.Register("DrawPointCard", new DrawPointCardCmdHandler());
        CommandDispatcher.Register("DrawSkillCard", new DrawSkillCardCmdHandler());
        CommandDispatcher.Register("ClearCardsToResolve", new ClearCardsToResolveCmdHandler());
        CommandDispatcher.Register("DiscardCard", new DiscardCardCmdHandler());
        CommandDispatcher.Register("ModifyPoint", new ModifyPointCmdHandler());
        CommandDispatcher.Register("MoveCard", new MoveCardCmdHandler());
        CommandDispatcher.Register("RevealCardsAndScore", new RevealCardsAndScoreCmdHandler());
        CommandDispatcher.Register("DetermineParticipants", new DetermineParticipantsCmdHandler());
        CommandDispatcher.Register("ValidateParticipants", new ValidateParticipantsCmdHandler());
        CommandDispatcher.Register("EndTurn", new EndTurnCmdHandler());
    }
}