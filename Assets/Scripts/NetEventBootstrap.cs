using Game.Domain;

public static class NetEventBootstrap
{
    public static void Init()
    {
        NetEventRegistry.Register<JoinOrCreateGameEvent>("JoinOrCreate");
        NetEventRegistry.Register<ChatEvent>("Chat");
        NetEventRegistry.Register<DrawCardEvent>("DrawCard");
        NetEventRegistry.Register<PlayCardEvent>("PlayCard");
    }
}

public static class CommandBootstrap
{
    public static void Init()
    {
        CommandRegistry.Register<JoinOrCreateGameCommand>("JoinOrCreate");
        CommandRegistry.Register<DrawCardCommand>("DrawCard");
        CommandRegistry.Register<PlayCardCommand>("PlayCard");
        CommandRegistry.Register<ChatCommand>("Chat");
    }
}