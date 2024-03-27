using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using strange.extensions.mediation.impl;

public class WinPopupMediator : EventMediator
{
    [Inject] public WinPopup WinPopup { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }

    private CancellationTokenSource _starsAnimCts = new CancellationTokenSource();

    private void Start()
    {
        var levelIndex = Array.IndexOf(LevelsCollectionProvider.Instance.Levels, LevelModel.LevelConfig);
        var title = string.Format(Loc.Get(LocalizationGroupId.WinPopup, "title"), levelIndex < 0 ? string.Empty : (levelIndex + 1).ToString());
        WinPopup.SetTitle(title);
        WinPopup.SetInfo(Loc.Get(LocalizationGroupId.WinPopup, "info"));

        AnimateStarsAsync(LevelModel.GetAccuracyRate());

        Activate();
    }

    private void Activate()
    {
        WinPopup.MainMenuButtonClicked += OnMainClicked;
        WinPopup.NextLevelButtonClicked += OnNextLvlClicked;
    }

    private void OnNextLvlClicked()
    {
        Deactivate();
        dispatcher.Dispatch(MediatorEvents.UI_WIN_POPUP_NEXT_LEVEL_CLICKED);
    }

    private void OnMainClicked()
    {
        Deactivate();
        dispatcher.Dispatch(MediatorEvents.UI_WIN_POPUP_MAIN_MENU_CLICKED);
    }

    private void OnDestroy()
    {
        _starsAnimCts.Cancel();

        Deactivate();
    }

    private void Deactivate()
    {
        WinPopup.MainMenuButtonClicked -= OnMainClicked;
        WinPopup.NextLevelButtonClicked -= OnNextLvlClicked;
    }

    private async void AnimateStarsAsync(int starsAmount)
    {
        await WinPopup.ShowTask;
        await UniTask.Delay(300, cancellationToken: _starsAnimCts.Token);

        for (var i = 0; i < starsAmount; i++)
        {
            if (_starsAnimCts.IsCancellationRequested) return;
            WinPopup.AnimateStar(i);
            PlayStarSound(i);
            await UniTask.Delay(500, cancellationToken: _starsAnimCts.Token);
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
