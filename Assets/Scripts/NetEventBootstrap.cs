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