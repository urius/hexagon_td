using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class StartWaveButtonViewMediator : EventMediator
{
    [Inject] public StartWaveButtonView StartWaveButtonView { get; set; }
    [Inject] public WaveModel WaveModel { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        StartWaveButtonView.OnClick += OnStartWaveClick;
        WaveModel.WaveStateChanged += OnWaveStateChanged;

        StartWaveButtonView.SetText(
            $"{Loc.Get(LocalizationGroupId.BottomPanel, "start_wave")}");
        //UpdateView();// not working here
    }

    private void OnDestroy()
    {
        StartWaveButtonView.OnClick -= OnStartWaveClick;
    }

    private void OnStartWaveClick()
    {
        dispatcher.Dispatch(MediatorEvents.UI_START_WAVE_CLICKED);
    }

    private void OnWaveStateChanged()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        if (WaveModel.WaveState == WaveState.InWave)
        {
            StartWaveButtonView.SetActiveAnimated(false);
        }
        else if (WaveModel.WaveState != WaveState.AfterLastWave)
        {
            StartWaveButtonView.SetActiveAnimated(true);
        }
    }
}
