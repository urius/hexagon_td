using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixerGroup MasterGroup;
    [SerializeField] private AudioMixerGroup MusicGroup;
    [SerializeField] private AudioMixerGroup SoundsGroup;
    [SerializeField] private MusicConfig[] MusicConfigs;
    [SerializeField] private SoundConfig[] SoundConfigs;

    private AudioSource _musicSource;
    private AudioSource _soundsSource;

    public void Play(MusicId musicId)
    {
        var clip = Array.Find(MusicConfigs, c => c.Id == musicId).AudioClip;
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void Play(SoundId soundId)
    {
        var clip = Array.Find(SoundConfigs, c => c.Id == soundId).AudioClip;
        _soundsSource.PlayOneShot(clip);
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void SetMusicVolume(float value)
    {
        _musicSource.volume = value;
    }

    public void SetSoundsVolume(float value)
    {
        _soundsSource.volume = value;
    }

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _soundsSource = gameObject.AddComponent<AudioSource>();

        _musicSource.outputAudioMixerGroup = MusicGroup;
        _soundsSource.outputAudioMixerGroup = SoundsGroup;
    }
}

[Serializable]
struct MusicConfig
{
    public MusicId Id;
    public AudioClip AudioClip;
}

[Serializable]
struct SoundConfig
{
    public SoundId Id;
    public AudioClip AudioClip;
}

public enum MusicId
{
    Menu_1,
    Game_1,
}

public enum SoundId
{
    ButtonClick_1,
}
