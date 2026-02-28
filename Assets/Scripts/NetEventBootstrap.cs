using Game.Domain;

public static class NetEventBootstrap
{
    public static void Init()
    {
        NetEventRegistry.Register<StartGameEvent>("StartGame");
        NetEventRegistry.Register<ChatEvent>("Chat");
        NetEventRegistry.Register<DrawCardEvent>("DrawCard");
        NetEventRegistry.Register<PlayCardEvent>("PlayCard");
    }
}

public static class CommandBootstrap
{
    public static void Init()
    {
        CommandRegistry.Register<JoinOrCreateCommand>("JoinOrCreate");
        CommandRegistry.Register<DrawCardCommand>("DrawCard");
        CommandRegistry.Register<PlayCardCommand>("PlayCard");
        CommandRegistry.Register<ChatCommand>("Chat");
    }
}