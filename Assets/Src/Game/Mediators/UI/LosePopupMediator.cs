using System;
using strange.extensions.mediation.impl;

public class LosePopupMediator : EventMediator
{
    [Inject] public LosePopup LosePopup { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public WaveModel WaveModel { get; set; }

    private void Start()
    {
        LosePopup.SetTitle(Loc.Get(LocalizationGroupId.LosePopup, "title"));
        LosePopup.SetInfo(Loc.Get(LocalizationGroupId.LosePopup, "info"));
        LosePopup.SetContinuePrice(LevelModel.GetContinueWavePrice());

        Activate();
    }

    private void Activate()
    {
        LosePopup.MainMenuButtonClicked += OnMainClicked;
        LosePopup.ContinueButtonClicked += OnContinueClicked;

        WaveModel.WaveStateChanged += OnWaveStateChanged;
    }

    private void Deactivate()
    {
        LosePopup.MainMenuButtonClicked -= OnMainClicked;
        LosePopup.ContinueButtonClicked -= OnContinueClicked;

        WaveModel.WaveStateChanged -= OnWaveStateChanged;
    }

    private void OnWaveStateChanged()
    {
        if (WaveModel.WaveState != WaveState.Terminated)
        {
            Deactivate();
            LosePopup.Hide();
        }
    }

    private void OnDestroy()
    {
        Deactivate();
    }

    private void OnMainClicked()
    {
        Deactivate();
        dispatcher.Dispatch(MediatorEvents.UI_LOSE_POPUP_MAIN_MENU_CLICKED);
    }

    private void OnContinueClicked()
    {
        if (PlayerGlobalModelHolder.Model.Gold >= LevelModel.GetContinueWavePrice())
        {
            dispatcher.Dispatch(MediatorEvents.UI_LOSE_POPUP_CONTINUE_CLICKED);
        }
        else
        {
            dispatcher.Dispatch(MediatorEvents.UI_GOLD_CLICKED);
        }
    }
}
