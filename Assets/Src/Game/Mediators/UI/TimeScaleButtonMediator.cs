using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class TimeScaleButtonMediator : EventMediator
{
    [Inject] public TimeScaleButtonView View { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }

    private void Start()
    {
        View.Clicked += OnChangeTimeScaleClicked;
        LevelModel.GameSpeedChanged += OnGameSpeedChanged;

        View.ShowSpeedMultiplier(LevelModel.TimeScale);
    }

    private void OnDestroy()
    {
        View.Clicked -= OnChangeTimeScaleClicked;
        LevelModel.GameSpeedChanged -= OnGameSpeedChanged;
    }

    private void OnGameSpeedChanged()
    {
        switch (LevelModel.TimeScale)
        {
            case 1:
                AudioManager.Instance.Play(SoundId.GameTime1);
                break;
            case 2:
                AudioManager.Instance.Play(SoundId.GameTime2);
                break;
            case 3:
                AudioManager.Instance.Play(SoundId.GameTime3);
                break;
        }
        View.ShowSpeedMultiplier(LevelModel.TimeScale);
    }

    private void OnChangeTimeScaleClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_TIME_SCALE_CHANGE_CLICKED);
    }
}
