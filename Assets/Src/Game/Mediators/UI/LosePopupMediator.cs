using System;
using strange.extensions.mediation.impl;

public class LosePopupMediator : EventMediator
{
    [Inject] public LosePopup LosePopup { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }

    private void Start()
    {
        LosePopup.SetTitle(Loc.Get(LocalizationGroupId.LosePopup, "title"));
        LosePopup.SetInfo(Loc.Get(LocalizationGroupId.LosePopup, "info"));

        Activate();
    }

    private void Activate()
    {
        LosePopup.MainMenuButtonClicked += OnMainClicked;
        LosePopup.RestartButtonClicked += OnRestartClicked;
    }

    private void Deactivate()
    {
        LosePopup.MainMenuButtonClicked -= OnMainClicked;
        LosePopup.RestartButtonClicked -= OnRestartClicked;
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

    private void OnRestartClicked()
    {
        Deactivate();
        dispatcher.Dispatch(MediatorEvents.UI_LOSE_POPUP_RESTART_LEVEL_CLICKED);
    }
}
