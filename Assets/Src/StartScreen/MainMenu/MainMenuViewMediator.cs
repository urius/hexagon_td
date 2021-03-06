﻿using strange.extensions.mediation.impl;

public class MainMenuViewMediator : EventMediator
{
    [Inject] public MainMenuView MainMenuView { get; set; }

    [Inject] public LocalizationProvider Loc { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        MainMenuView.PlayButtonClicked += OnPlayClicked;
        MainMenuView.HowToPlayButtonClicked += OnHowToPlayClicked;
        MainMenuView.SettingsButtonClicked += OnSettingsClicked;
        MainMenuView.SpecialThanksClicked += OnSpecialThanksClicked;

        MainMenuView.SetTexts(
            Loc.Get(LocalizationGroupId.StartScreen, "main_menu_play"),
            Loc.Get(LocalizationGroupId.StartScreen, "main_menu_settings"),
            Loc.Get(LocalizationGroupId.StartScreen, "main_menu_how_to_play"),
            Loc.Get(LocalizationGroupId.StartScreen, "main_menu_special_thanks")
            );
    }

    private void OnPlayClicked()
    {
        MainMenuView.HideAnimated();
        dispatcher.Dispatch(MediatorEvents.UI_SS_PLAY_CLICKED);
    }

    private void OnHowToPlayClicked()
    {
        MainMenuView.HideAnimated();
        dispatcher.Dispatch(MediatorEvents.UI_SS_HOW_TO_PLAY_CLICKED);
    }

    private void OnSettingsClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_SETTINGS_CLICKED);
    }

    private void OnSpecialThanksClicked()
    {
        MainMenuView.HideAnimated();
        dispatcher.Dispatch(MediatorEvents.UI_SS_SPECIAL_THANKS_CLICKED);
    }
}
