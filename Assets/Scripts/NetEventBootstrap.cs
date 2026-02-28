using Game.Domain;

public static class NetEventBootstrap
{
    public static void Init()
    {
        EventDispatcher.Register("JoinOrCreateGame", new JoinOrCreateGameEventHandler());
        EventDispatcher.Register("Chat", new ChatEventHandler());
        EventDispatcher.Register("DrawCard", new DrawCardEventHandler());
        EventDispatcher.Register("ReadyToPlaySkillCard", new ReadyToPlaySkillCardEventHandler());


        CommandDispatcher.Register("JoinOrCreateGame", new JoinOrCreateGameCmdHandler());
        CommandDispatcher.Register("Chat", new ChatCmdHandler());
        CommandDispatcher.Register("DrawCard", new DrawCardCmdHandler());
        CommandDispatcher.Register("ReadyToPlaySkillCard", new ReadyToPlaySkillCardCmdHandler());
    }
}