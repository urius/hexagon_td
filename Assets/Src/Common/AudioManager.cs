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
    private AudioSource _musicSecondarySource;
    private AudioSource _primarySoundsSource;
    private float _musicVolume;
    private MusicId _currentPlayingMusicId;
    private bool _inWaveMusicMode;
    private readonly List<AudioSource> _additionalAudioSources = new List<AudioSource>();
    private readonly Dictionary<SoundId, int> _soundsPlayTimeFrames = new Dictionary<SoundId, int>();

    public void Play(MusicId musicId)
    {
        _currentPlayingMusicId = musicId;

        var musicConfig = Array.Find(MusicConfigs, c => c.Id == musicId);
        _musicSource.clip = musicConfig.AudioClip;
        _musicSource.Play();

        _inWaveMusicMode = false;
        _musicSecondarySource.volume = 0;
        if (musicConfig.SecondaryAudioClip != null)
        {
            _musicSecondarySource.clip = musicConfig.SecondaryAudioClip;
            _musicSecondarySource.Play();
        }
    }

    public async Task FadeInAndPlayMusicIfNotPlayedAsync(MusicId musicId)
    {
        var playingMusic = GetPlayingMusic();
        if (GetPlayingMusic() != musicId)
        {
            if (playingMusic != MusicId.None)
            {
                await FadeOutAndStopMusicAsync();
            }
            await PlayAsync(musicId);
        }
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

    public Task PlayAsync(MusicId musicId)
    {
        _musicSource.volume = 0;
        Play(musicId);

        return FadeInAsync(_musicSource);
    }

    public void SetInWaveMusicMode(bool isInWaveEnabled)
    {
        _inWaveMusicMode = isInWaveEnabled;
        var clip = Array.Find(MusicConfigs, c => c.Id == _currentPlayingMusicId).SecondaryAudioClip;
        if (clip != null)
        {
            if (isInWaveEnabled)
            {
                FadeInAsync(_musicSecondarySource);
            }
            else
            {
                FadeOutAsync(_musicSecondarySource);
            }
        }
    }

    public async Task FadeOutAndStopMusicAsync()
    {
        var mainMusicFadeOutTask = FadeOutAsync(_musicSource);
        var secondaryMusicFadeOutTask = FadeOutAsync(_musicSecondarySource);

        await Task.WhenAll(mainMusicFadeOutTask, secondaryMusicFadeOutTask);

        _musicSource.Stop();
        _musicSecondarySource.Stop();
        _musicSource.volume = _musicVolume;
        _musicSecondarySource.volume = 0;
    }

    public void Play(SoundId soundId)
    {
        if (!_soundsPlayTimeFrames.ContainsKey(soundId))
        {
            _soundsPlayTimeFrames.Add(soundId, Time.frameCount);
        }
        else
        {
            if (Time.frameCount - _soundsPlayTimeFrames[soundId] < 4)
            {
                return;
            }
            _soundsPlayTimeFrames[soundId] = Time.frameCount;
        }
        var clip = Array.Find(SoundConfigs, c => c.Id == soundId).AudioClip;
        _primarySoundsSource.PlayOneShot(clip);
    }

    public void PlayOnSource(AudioSource source, SoundId soundId)
    {
        var clip = Array.Find(SoundConfigs, c => c.Id == soundId).AudioClip;
        source.clip = clip;
        source.Play();
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void SetMusicVolume(float value)
    {
        _musicVolume = value;
        _musicSource.volume = _musicVolume;
        if (_inWaveMusicMode)
        {
            _musicSecondarySource.volume = _musicVolume;
        }
    }

    public void SetSoundsVolume(float value)
    {
        _primarySoundsSource.volume = value;
        _additionalAudioSources.ForEach(s => s.volume = value);
    }

    public AudioSource CreateAudioSource()
    {
        var source = gameObject.AddComponent<AudioSource>();

        _additionalAudioSources.Add(source);

        return source;
    }

    public bool RemoveAudioSource(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        var result = _additionalAudioSources.Remove(source);
        Destroy(source);

        return result;
    }

    public float GetSoundLength(SoundId soundId)
    {
        var clip = Array.Find(SoundConfigs, c => c.Id == soundId).AudioClip;
        return clip.length;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSecondarySource = gameObject.AddComponent<AudioSource>();
        _musicSecondarySource.loop = true;
        _primarySoundsSource = gameObject.AddComponent<AudioSource>();

        _musicSource.outputAudioMixerGroup = MusicGroup;
        _primarySoundsSource.outputAudioMixerGroup = SoundsGroup;
    }

    private Task FadeInAsync(AudioSource source)
    {
        var tsc = new TaskCompletionSource<bool>();
        IEnumerator FadeIn()
        {
            var startVolume = source.volume;
            var maxI = 20;
            for (var i = 0; i < maxI; i++)
            {
                source.volume = Mathf.Lerp(startVolume, _musicVolume, (float)i / maxI);
                yield return new WaitForSecondsRealtime(0.05f);
            }
            tsc.TrySetResult(true);
        }

        StartCoroutine(FadeIn());

        return tsc.Task;
    }

    private Task FadeOutAsync(AudioSource source)
    {
        var tsc = new TaskCompletionSource<bool>();
        IEnumerator FadeOut()
        {
            var startVolume = source.volume;
            var maxI = 10;
            for (var i = 0; i <= maxI; i++)
            {
                source.volume = Mathf.Lerp(startVolume, 0, (float)i / maxI);
                yield return new WaitForSecondsRealtime(0.05f);
            }

            tsc.TrySetResult(true);
        }

        StartCoroutine(FadeOut());

        return tsc.Task;
    }
}

[Serializable]
struct MusicConfig
{
    public MusicId Id;
    public AudioClip AudioClip;
    public AudioClip SecondaryAudioClip;
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
    Menu_1,
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
    UnitDestroyed,
    LevelComplete,
    LevelLose,
    WinStar_1,
    WinStar_2,
    UnitDestroyedOnBase,
    ClickDefault,
    ClickStartLevel,
    ClickStartWave,
    PortalEnter,
    PortalExit,
    ExtraMoneyCell,
    SellTurret,
    ClickOnTurret,
    TurretUpdgrade1,
    TurretUpdgrade2,
    TurretUpdgrade3,
    TurretPlaced,
    TurretUnableToPlace,
    InsufficientFunds,
    WaveFinished,
    GameTime1,
    GameTime2,
    GameTime3,
    ChooseTurretFouBuild,
    BuildTurretCanceled,
}
