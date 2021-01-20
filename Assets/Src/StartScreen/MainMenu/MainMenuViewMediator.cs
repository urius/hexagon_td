using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class MainMenuViewMediator : EventMediator
{
    [Inject] public MainMenuView MainMenuView { get; set; }

    [Inject] public LocalizationProvider Loc { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        MainMenuView.PlayButtonClicked += OnPlayClicked;
        MainMenuView.HowToPlayButtonClicked += OnHowToPlayClicked;

        MainMenuView.SetTexts(
            Loc.Get(LocalizationGroupId.StartScreen, "main_menu_play"),
            Loc.Get(LocalizationGroupId.StartScreen, "main_menu_settings"),
            Loc.Get(LocalizationGroupId.StartScreen, "main_menu_how_to_play"));
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
}
