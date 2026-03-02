using Newtonsoft.Json;
using UnityEngine;

public static class JsonBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            //SerializationBinder = new KnownTypesBinder(),
            //NullValueHandling = NullValueHandling.Ignore,
        };
    }
}