using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public MusicId GetPlayingMusic()
    {
        var result = MusicId.None;

        if (_musicSource.isPlaying)
        {
            result = Array.Find(MusicConfigs, c => c.AudioClip == _musicSource.clip).Id;
        }

        return result;
    }

    public Task PLayAsync(MusicId musicId)
    {
        var musicVolume = _musicSource.volume;
        _musicSource.volume = 0;
        Play(musicId);

        var tsc = new TaskCompletionSource<bool>();
        IEnumerator FadeIn()
        {
            var maxI = 20;
            for (var i = 0; i < maxI; i++)
            {
                _musicSource.volume = Mathf.Lerp(0, musicVolume, (float)i / maxI);
                yield return new WaitForSecondsRealtime(0.05f);
            }
            tsc.TrySetResult(true);
        }

        StartCoroutine(FadeIn());

        return tsc.Task;
    }

    public Task FadeOutAndStopMusicAsync()
    {
        var musicVolume = _musicSource.volume;

        var tsc = new TaskCompletionSource<bool>();
        IEnumerator FadeOut()
        {
            var maxI = 20;
            for (var i = 0; i < maxI; i++)
            {
                _musicSource.volume = Mathf.Lerp(musicVolume, 0, (float)i / maxI);
                yield return new WaitForSecondsRealtime(0.05f);
            }

            _musicSource.Stop();
            _musicSource.volume = musicVolume;

            tsc.TrySetResult(true);
        }

        StartCoroutine(FadeOut());

        return tsc.Task;
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
    None,
    Game_1,
}

public enum SoundId
{
    None,
    Gun_1,
    Gun_2,
    Gun_3,
    Laser_1,
    Laser_2,
    Laser_3,
    Rocket_1,
    Rocket_2,
    Rocket_3,
    SlowField_1,
    SlowField_2,
    SlowField_3,
}
