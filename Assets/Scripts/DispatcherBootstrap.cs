using Game.Domain;

public static class DispatcherBootstrap
{
    public static void Init()
    {
        EventDispatcher.Register("JoinOrCreateMatch", new JoinOrCreateMatchEventHandler());
        EventDispatcher.Register("StartMatch", new StartMatchEventHandler());
        EventDispatcher.Register("StartGame", new StartGameEventHandler());
        EventDispatcher.Register("AssignRoles", new AssignRolesEventHandler());
        EventDispatcher.Register("Place1Bet", new Place1BetEventHandler());
        EventDispatcher.Register("ConfirmBet", new ConfirmBetEventHandler());
        EventDispatcher.Register("StartTurn", new StartTurnEventHandler());
        EventDispatcher.Register("GetGameState", new GetGameStateEventHandler());
        EventDispatcher.Register("GetCtx", new GetCtxEventHandler());
        EventDispatcher.Register("Chat", new ChatEventHandler());
        EventDispatcher.Register("PlayAnimation", new PlayAnimationEventHandler());
        EventDispatcher.Register("ValidateActionPoint", new ValidateActionPointEventHandler());
        EventDispatcher.Register("SpendActionPoint", new SpendActionPointEventHandler());
        EventDispatcher.Register("DrawPointCard", new DrawPointCardEventHandler());
        EventDispatcher.Register("DrawSkillCard", new DrawSkillCardEventHandler());
        EventDispatcher.Register("DrawPointCardToResolve", new DrawPointCardToResolveEventHandler());
        EventDispatcher.Register("ClearCardsToResolve", new ClearCardsToResolveEventHandler());
        EventDispatcher.Register("PlayResolveAnim", new PlayResolveAnimEventHandler());
        EventDispatcher.Register("DiscardCard", new DiscardCardEventHandler());
        EventDispatcher.Register("ModifyPoint", new ModifyPointEventHandler());
        EventDispatcher.Register("MoveCard", new MoveCardEventHandler());
        EventDispatcher.Register("RevealCardsAndScore", new RevealCardsAndScoreEventHandler());
        EventDispatcher.Register("DetermineParticipants", new DetermineParticipantsEventHandler());
        EventDispatcher.Register("ValidateParticipants", new ValidateParticipantsEventHandler());
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
        CommandDispatcher.Register("PlayAnimation", new PlayAnimationCmdHandler());
        CommandDispatcher.Register("ValidateActionPoint", new ValidateActionPointCmdHandler());
        CommandDispatcher.Register("SpendActionPoint", new SpendActionPointCmdHandler());
        CommandDispatcher.Register("DrawPointCard", new DrawPointCardCmdHandler());
        CommandDispatcher.Register("DrawSkillCard", new DrawSkillCardCmdHandler());
        CommandDispatcher.Register("ClearCardsToResolve", new ClearCardsToResolveCmdHandler());
        CommandDispatcher.Register("PlayResolveAnim", new PlayResolveAnimCmdHandler());
        CommandDispatcher.Register("DiscardCard", new DiscardCardCmdHandler());
        CommandDispatcher.Register("ModifyPoint", new ModifyPointCmdHandler());
        CommandDispatcher.Register("MoveCard", new MoveCardCmdHandler());
        CommandDispatcher.Register("RevealCardsAndScore", new RevealCardsAndScoreCmdHandler());
        CommandDispatcher.Register("DetermineParticipants", new DetermineParticipantsCmdHandler());
        CommandDispatcher.Register("ValidateParticipants", new ValidateParticipantsCmdHandler());
        CommandDispatcher.Register("EndTurn", new EndTurnCmdHandler());
    }
}