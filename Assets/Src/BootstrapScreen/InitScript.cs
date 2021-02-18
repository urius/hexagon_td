using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitScript : MonoBehaviour
{
    [SerializeField] private PlayerGlobalModelHolder _playerGlobalModelHolder;
    [SerializeField] private LevelsCollectionProvider _levelsCollectionProvider;
    [SerializeField] private LevelConfigProvider _levelConfigProvider;
    [SerializeField] private Text _loadingText;
    [SerializeField] private RectTransform _canvasTransform;
    [SerializeField] private UIPrefabsConfig _uiPrefabsConfig;

    private void Start()
    {
        _loadingText.text = LocalizationProvider.Instance.Get(LocalizationGroupId.BootstrapScreen, "loading");

        _levelConfigProvider.SetCurrentLevelConfig(null);

        StartLoadSequence().Forget();
    }

    private async UniTaskVoid StartLoadSequence()
    {
        //AskPermissions();
        if (await LoadOrCreateData())
        {
            SetupAudioManager();
            LoadScene();
        }
        else
        {
            await UniTask.DelayFrame(10);

            if (Application.isPlaying)
            {
                StartLoadSequence().Forget();
            }        
        }
    }

    private void SetupAudioManager()
    {
        var playerModel = _playerGlobalModelHolder.PlayerGlobalModel;
        AudioManager.Instance.SetMasterVolume(playerModel.AudioVolume);
        AudioManager.Instance.SetMusicVolume(playerModel.MusicVolume);
        AudioManager.Instance.SetSoundsVolume(playerModel.SoundsVolume);
    }

    private async UniTask<bool> LoadOrCreateData()
    {
        _loadingText.text = "load data";

        var id = SystemInfo.deviceUniqueIdentifier;
        var result = await new LoadDataCommand().ExecuteAsync(id, _canvasTransform);

        return result;
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
