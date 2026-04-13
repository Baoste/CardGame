using Newtonsoft.Json;
using UnityEngine;

namespace Game.Domain
{
    public static class GameConfigLoader
    {
        private static GameConfig config;

        public static GameConfig Config
        {
            get
            {
                if (config == null)
                {
                    Load();
                }
                return config;
            }
        }

        private static void Load()
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "GameConfig.json");
            string json = System.IO.File.ReadAllText(filePath);
            config = JsonConvert.DeserializeObject<GameConfig>(json);
        }
    }
}