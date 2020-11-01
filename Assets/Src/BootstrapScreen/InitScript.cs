using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitScript : MonoBehaviour
{
    [SerializeField] private PlayerGlobalModelHolder _playerGlobalModelHolder;
    [SerializeField] private DeafultPlayerGlobalModelProvider _deafultPlayerGlobalModelProvider;
    [SerializeField] private LevelsCollectionProvider _levelsCollectionProvider;
    [SerializeField] private LocalizationProvider _localizationProvider;
    [SerializeField] private Text _loadingText;

    private void Start()
    {
        _loadingText.text = _localizationProvider.Get(LocalizationGroupId.BootstrapScreen, "loading");

        LoadOrCreateData();
    }

    private async void LoadOrCreateData()
    {
        if (!PlayerGlobalModel.TryLoad(out var playerGlobalModel))
        {
            playerGlobalModel = new PlayerGlobalModel(_deafultPlayerGlobalModelProvider.PlayerGlobalModel);
        }
        playerGlobalModel.AdjustLevelsAmount(_levelsCollectionProvider.Levels.Length);

        _playerGlobalModelHolder.SetModel(playerGlobalModel);

        await LoadSceneHelper.LoadSceneAdditiveAsync(SceneNames.MainMenu);
        SceneManager.UnloadSceneAsync(SceneNames.Bootstrap, UnloadSceneOptions.None);
    }
}
