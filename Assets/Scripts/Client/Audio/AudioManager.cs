using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Database")]
    [SerializeField] private AudioDatabase database;

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource uiSource;

    [Header("SFX Pool")]
    [SerializeField] private int sfxPoolSize = 16;
    [SerializeField] private AudioMixerGroup sfxGroup;

    private AudioSource[] sfxSources;
    private Dictionary<string, AudioSource> playingSfxSources;
    private int sfxIndex;

    private Coroutine bgmFadeCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        playingSfxSources = new Dictionary<string, AudioSource>();

        Instance = this;
        DontDestroyOnLoad(gameObject);

        database.Init();
        InitSFXPool();
    }

    private void InitSFXPool()
    {
        sfxSources = new AudioSource[sfxPoolSize];

        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject obj = new GameObject($"SFX_Source_{i}");
            obj.transform.SetParent(transform);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = sfxGroup;

            sfxSources[i] = source;
        }
    }

    public void Play(string id)
    {
        var config = database.Get(id);
        if (config == null || config.clip == null)
            return;

        AudioSource source = null;
        switch (config.type)
        {
            case AudioType.BGM:
                PlayBGM(id);
                break;
            case AudioType.SFX:
                source = sfxSources[sfxIndex];
                sfxIndex = (sfxIndex + 1) % sfxSources.Length;
                break;
            case AudioType.UI:
                source = uiSource;
                break;
            case AudioType.Ambience:
                source = ambienceSource;
                break;
        }

        if (source)
        {
            ApplyConfig(source, config);

            source.Play();
            if (config.loop)
            {
                playingSfxSources[id] = source;
            }
        }
    }

    public void Stop(string id)
    {
        if (playingSfxSources.TryGetValue(id, out AudioSource source))
        {
            source.Stop();
            playingSfxSources.Remove(id);
        }
    }

    public void PlayBGM(string id, float fadeTime = 1f)
    {
        var config = database.Get(id);
        if (config == null || config.clip == null)
            return;

        if (bgmSource.clip == config.clip[0])
            return;

        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(FadeBGM(config, fadeTime));
    }

    private IEnumerator FadeBGM(AudioClipConfig config, float fadeTime)
    {
        float startVolume = bgmSource.volume;

        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }

        ApplyConfig(bgmSource, config);
        bgmSource.loop = true;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float targetVolume = config.volume;

        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeTime);
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }

    private void ApplyConfig(AudioSource source, AudioClipConfig config)
    {
        if (config.randomClip)
            source.clip = config.clip[Random.Range(0, config.clip.Length)];
        else
            source.clip = config.clip[0];

        float finalVolume = config.volume;
        if (config.randomVolume)
            finalVolume *= Random.Range(config.volumeRange.x, config.volumeRange.y);

        float finalPitch = config.pitch;
        if (config.randomPitch)
            finalPitch *= Random.Range(config.pitchRange.x, config.pitchRange.y);

        source.volume = finalVolume;
        source.pitch = finalPitch;
        source.loop = config.loop;
    }

    public void SetMasterVolume(float value)
    {
        SetMixerVolume("MasterVolume", value);
    }

    public void SetBGMVolume(float value)
    {
        SetMixerVolume("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SetMixerVolume("SFXVolume", value);
    }

    private void SetMixerVolume(string parameter, float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        audioMixer.SetFloat(parameter, Mathf.Log10(value) * 20f);
    }
}