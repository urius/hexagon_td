using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitScript : MonoBehaviour
{
    [SerializeField] private PlayerGlobalModelHolder _playerGlobalModelHolder;
    [SerializeField] private DeafultPlayerGlobalModelProvider _deafultPlayerGlobalModelProvider;
    [SerializeField] private LevelsCollectionProvider _levelsCollectionProvider;
    [SerializeField] private LevelConfigProvider _levelConfigProvider;
    [SerializeField] private LocalizationProvider _localizationProvider;
    [SerializeField] private Text _loadingText;

    private void Start()
    {
        _loadingText.text = _localizationProvider.Get(LocalizationGroupId.BootstrapScreen, "loading");

        _levelConfigProvider.SetCurrentLevelConfig(null);

        //AskPermissions();
        LoadOrCreateData();
        SetupAudioManager();
        LoadScene();
    }

    private void SetupAudioManager()
    {
        var playerModel = _playerGlobalModelHolder.PlayerGlobalModel;
        AudioManager.Instance.SetMasterVolume(playerModel.AudioVolume);
        AudioManager.Instance.SetMusicVolume(playerModel.MusicVolume);
        AudioManager.Instance.SetSoundsVolume(playerModel.SoundsVolume);
    }

    private void LoadOrCreateData()
    {
        _loadingText.text = "load data";

        Debug.Log("PlayerGlobalModel load started");
        if (!PlayerGlobalModel.TryLoad(out var playerGlobalModel))
        {
           // _loadingText.text = "create new model";
            Debug.Log("_deafultPlayerGlobalModelProvider is null:" + (_deafultPlayerGlobalModelProvider == null));
            playerGlobalModel = new PlayerGlobalModel(_deafultPlayerGlobalModelProvider.PlayerGlobalModel);
        }

        //_loadingText.text = "AdjustLevelsAmount";

        Debug.Log("_levelsCollectionProvider is null:" + (_levelsCollectionProvider == null));
        Debug.Log("_levelsCollectionProvider.Levels is null:" + (_levelsCollectionProvider.Levels == null));
        playerGlobalModel.AdjustLevelsAmount(_levelsCollectionProvider.Levels.Length);

        Debug.Log("_playerGlobalModelHolder is null:" + (_playerGlobalModelHolder == null));
        _playerGlobalModelHolder.SetModel(playerGlobalModel);
    }

    private void AskPermissions()
    {
#if UNITY_ANDROID
        while (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
#endif
    }

    private async void LoadScene()
    {
        _loadingText.text = "load scene";

        await LoadSceneHelper.LoadSceneAdditiveAsync(SceneNames.MainMenu);
        SceneManager.UnloadSceneAsync(SceneNames.Bootstrap, UnloadSceneOptions.None);
    }
}
