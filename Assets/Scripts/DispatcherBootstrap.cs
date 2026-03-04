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
        EventDispatcher.Register("DrawPointCard", new DrawPointCardEventHandler());
        EventDispatcher.Register("DrawSkillCard", new DrawSkillCardEventHandler());
        EventDispatcher.Register("DiscardCard", new DiscardCardEventHandler());
        EventDispatcher.Register("ReadyToPlaySkillCardEffect", new ReadyToPlaySkillCardEffectEventHandler());
        EventDispatcher.Register("ValidateSkillCard", new ValidateSkillCardEventHandler());
        EventDispatcher.Register("EndTurn", new EndTurnEventHandler());

        CommandDispatcher.Register("LeaveGame", new LeaveGameCmdHandler());
        CommandDispatcher.Register("JoinOrCreateGame", new JoinOrCreateGameCmdHandler());
        CommandDispatcher.Register("StartGame", new StartGameCmdHandler());
        CommandDispatcher.Register("StartTurn", new StartTurnCmdHandler());
        CommandDispatcher.Register("GetGameState", new GetGameStateCmdHandler());
        CommandDispatcher.Register("GetCtx", new GetCtxCmdHandler());
        CommandDispatcher.Register("Chat", new ChatCmdHandler());
        CommandDispatcher.Register("DrawPointCard", new DrawPointCardCmdHandler());
        CommandDispatcher.Register("DrawSkillCard", new DrawSkillCardCmdHandler());
        CommandDispatcher.Register("DiscardCard", new DiscardCardCmdHandler());
        CommandDispatcher.Register("ReadyToPlaySkillCardEffect", new ReadyToPlaySkillCardEffectCmdHandler());
        CommandDispatcher.Register("ValidateSkillCard", new ValidateSkillCardCmdHandler());
        CommandDispatcher.Register("EndTurn", new EndTurnCmdHandler());
    }
}