using UnityEngine;

public enum AudioType
{
    BGM,
    SFX,
    UI,
    Ambience
}

[CreateAssetMenu(menuName = "Game/Audio/Audio Clip Config")]
public class AudioClipConfig : ScriptableObject
{
    public string id;
    public AudioClip[] clip;

    public AudioType type;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.5f, 2f)]
    public float pitch = 1f;

    public bool loop;

    [Header("Random")]
    public bool randomClip;

    public bool randomPitch;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    public bool randomVolume;
    public Vector2 volumeRange = new Vector2(0.9f, 1f);
}
