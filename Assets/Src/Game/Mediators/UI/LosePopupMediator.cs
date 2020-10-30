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

        LosePopup.MainMenuButtonClicked += OnMainClicked;
        LosePopup.RestartButtonClicked += OnRestartClicked;
    }

    private void OnDestroy()
    {
        LosePopup.MainMenuButtonClicked -= OnMainClicked;
        LosePopup.RestartButtonClicked -= OnRestartClicked;
    }

    private void OnMainClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_LOSE_POPUP_MAIN_MENU_CLICKED);
    }

    private void OnRestartClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_LOSE_POPUP_RESTART_LEVEL_CLICKED);
    }
}
