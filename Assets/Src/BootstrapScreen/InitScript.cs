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
    [SerializeField] private DeafultPlayerGlobalModelProvider _deafultPlayerGlobalModelProvider;
    [SerializeField] private LevelsCollectionProvider _levelsCollectionProvider;
    [SerializeField] private LevelConfigProvider _levelConfigProvider;
    [SerializeField] private LocalizationProvider _localizationProvider;
    [SerializeField] private Text _loadingText;
    [SerializeField] private RectTransform _canvasTransform;
    [SerializeField] private UIPrefabsConfig _uiPrefabsConfig;

    private async UniTaskVoid Start()
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
        };
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
        var result = await NetworkManager.GetUserDataAsync(id);

        if (result.IsSuccess)
        {
            if (!result.Result.IsError && false)
            {
                var playerGlobalModel = result.Result.payload;
                playerGlobalModel.AdjustLevelsAmount(_levelsCollectionProvider.Levels.Length);
                _playerGlobalModelHolder.SetModel(playerGlobalModel);

                return true;
            } else
            {
                var errorPopupGo = Instantiate(_uiPrefabsConfig.ErrorPopupPrefab, _canvasTransform);
                var errorPopup = errorPopupGo.GetComponent<ErrorPopup>();
                errorPopup.SetTexts("Error", $"Retrieve data error\ncode: {result.Result.error.code}", "Try again");
                //TODO show popup with data error (contact with developer)
                await errorPopup.LifeTimeTask;

                StartLoadSequence().Forget();
            }
        } else
        {
            //TODO show popup with server error (try again later)
        }

        return false;
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
