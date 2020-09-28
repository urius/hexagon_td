using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using strange.extensions.mediation.impl;
using UnityEngine;

public class UiCanvasViewMediator : EventMediator
{
    [Inject] public UICanvasView UICanvasView { get; set; }
    [Inject] public WaveModel WaveModel { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public UIPrefabsConfig UIPrefabsConfig { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }

    private CancellationTokenSource _infoPanelCts = new CancellationTokenSource();

    public override async void OnRegister()
    {
        base.OnRegister();

        await LevelModel.StartLevelTask;

        WaveModel.WaveStateChanged += OnWaveStateChanged;
        UpdateGeneralInfo();
    }

    private void OnDestroy()
    {
        WaveModel.WaveStateChanged -= OnWaveStateChanged;

        _infoPanelCts.Cancel();
        _infoPanelCts.Dispose();
    }

    private void UpdateGeneralInfo()
    {
        switch (WaveModel.WaveState)
        {
            case WaveState.BeforeFirstWave:
                ShowGeneralInfo("before_first_wave", 200);
                break;
            case WaveState.InWave:
                ShowGeneralInfo("wave_started");
                break;
            case WaveState.BetweenWaves:
                ShowGeneralInfo("wave_finished", 800);
                break;
            case WaveState.AfterLastWave:
                ShowGeneralInfo("all_waves_finished", 800);
                break;
            case WaveState.Terminated:
                ShowGeneralInfo("defeat", 1000, Color.red);
                break;
        }
    }

    private void OnWaveStateChanged()
    {
        UpdateGeneralInfo();
    }

    private async void ShowGeneralInfo(string infoKey, int delayMs = 0, Color? textColor = null)
    {
        if (!_infoPanelCts.IsCancellationRequested)
        {
            _infoPanelCts.Cancel();
            _infoPanelCts.Dispose();
        }
        _infoPanelCts = new CancellationTokenSource();
        var stopToken = _infoPanelCts.Token;

        if (delayMs > 0 && !stopToken.IsCancellationRequested) await Task.Delay(delayMs, stopToken).ContinueWith(_ => { });
        var infoStr = Loc.Get(LocalizationGroupId.GeneralInfoPanel, infoKey);
        var infoPanelGo = Instantiate(UIPrefabsConfig.GeneralInfoPanelPrefab, UICanvasView.transform);
        var infoPanel = infoPanelGo.GetComponent<GeneralInfoPanelView>();

        if (textColor != null)
        {
            infoPanel.SetTextColor(textColor.Value);
        }
        await infoPanel.ShowAsync();
        if (!stopToken.IsCancellationRequested) await infoPanel.SetTextAsync(infoStr, stopToken);
        await Task.Delay(1000, stopToken).ContinueWith(_ => { });
        await infoPanel.HideAsync();

        Destroy(infoPanelGo);
    }
}
