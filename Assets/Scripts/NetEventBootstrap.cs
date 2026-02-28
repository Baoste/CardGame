using Game.Domain;

public static class NetEventBootstrap
{
    public static void Init()
    {
        EventDispatcher.Register("JoinOrCreateGame", new JoinOrCreateGameEventHandler());
        EventDispatcher.Register("Chat", new ChatEventHandler());
        EventDispatcher.Register("DrawCard", new DrawCardEventHandler());
        EventDispatcher.Register("ReadyToPlaySkillCard", new ReadyToPlaySkillCardEventHandler());
        EventDispatcher.Register("PlaySkillCardWithTarget", new PlaySkillCardWithTargetEventHandler());


        CommandDispatcher.Register("JoinOrCreateGame", new JoinOrCreateGameCmdHandler());
        CommandDispatcher.Register("Chat", new ChatCmdHandler());
        CommandDispatcher.Register("DrawCard", new DrawCardCmdHandler());
        CommandDispatcher.Register("ReadyToPlaySkillCard", new ReadyToPlaySkillCardCmdHandler());
        CommandDispatcher.Register("PlaySkillCardWithTarget", new PlaySkillCardWithTargetCmdHandler());
    }
}