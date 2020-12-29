using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using strange.extensions.mediation.impl;
using UnityEngine;

public class WinPopupMediator : EventMediator
{
    [Inject] public WinPopup WinPopup { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }
    [Inject] public LevelsCollectionProvider LevelsCollectionProvider { get; set; }
    [Inject] public LevelConfigProvider LevelConfigProvider { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }

    private CancellationTokenSource _starsAnimCts = new CancellationTokenSource();

    private void Start()
    {
        var levelIndex = Array.IndexOf(LevelsCollectionProvider.Levels, LevelConfigProvider.LevelConfig);
        var title = string.Format(Loc.Get(LocalizationGroupId.WinPopup, "title"), levelIndex < 0 ? string.Empty : (levelIndex + 1).ToString());
        WinPopup.SetTitle(title);
        WinPopup.SetInfo(Loc.Get(LocalizationGroupId.WinPopup, "info"));

        AnimateStarsAsync(LevelModel.GetAccuracyRate());

        WinPopup.MainMenuButtonClicked += OnMainClicked;
        WinPopup.NextLevelButtonClicked += OnNextLvlClicked;
    }

    private void OnMainClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_WIN_POPUP_MAIN_MENU_CLICKED);
    }

    private void OnNextLvlClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_WIN_POPUP_NEXT_LEVEL_CLICKED);
    }

    private void OnDestroy()
    {
        _starsAnimCts.Cancel();

        WinPopup.MainMenuButtonClicked -= OnMainClicked;
        WinPopup.NextLevelButtonClicked -= OnNextLvlClicked;
    }

    private async void AnimateStarsAsync(int starsAmount)
    {
        await WinPopup.ShowTask;
        await Task.Delay(300, _starsAnimCts.Token);

        for (var i = 0; i < starsAmount; i++)
        {
            if (_starsAnimCts.IsCancellationRequested) return;
            WinPopup.AnimateStar(i);
            PlayStarSound(i);
            await Task.Delay(500, _starsAnimCts.Token);
        }
    }

    private void PlayStarSound(int index)
    {
        if (index < 2)
        {
            AudioManager.Instance.Play(SoundId.WinStar_1);
        } else
        {
            AudioManager.Instance.Play(SoundId.WinStar_2);
        }
    }
}
