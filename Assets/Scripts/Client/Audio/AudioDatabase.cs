using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Game/Audio/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    public List<AudioClipConfig> clips = new();

    private Dictionary<string, AudioClipConfig> map;

    public void Init()
    {
        map = new Dictionary<string, AudioClipConfig>();

        foreach (var config in clips)
        {
            if (config == null || string.IsNullOrEmpty(config.id))
                continue;

            if (!map.TryAdd(config.id, config))
                Debug.LogWarning($"Duplicate audio id: {config.id}", config);
        }
    }

    public AudioClipConfig Get(string id)
    {
        if (map == null)
            Init();

        if (map.TryGetValue(id, out var config))
            return config;

        Debug.LogWarning($"Audio id not found: {id}");
        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("Load Audio Configs From Folder")]
    private void LoadAudioConfigsFromFolder()
    {
        clips.Clear();

        string folderPath = "Assets/Audio/AudioClips";
        string[] guids = AssetDatabase.FindAssets("t:AudioClipConfig", new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioClipConfig config = AssetDatabase.LoadAssetAtPath<AudioClipConfig>(path);

            if (config != null)
                clips.Add(config);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        Debug.Log($"Loaded {clips.Count} audio configs from {folderPath}", this);
    }
#endif
}