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

    private async void UpdateGeneralInfo()
    {
        string text;
        switch (WaveModel.WaveState)
        {
            case WaveState.BeforeFirstWave:
                text = Loc.Get(LocalizationGroupId.GeneralInfoPanel, "before_first_wave");
                await ShowGeneralInfo(text, 200);
                break;
            case WaveState.InWave:
                text = String.Format(Loc.Get(LocalizationGroupId.GeneralInfoPanel, "wave_started"), WaveModel.WaveIndex + 1);
                await ShowGeneralInfo(text);
                break;
            case WaveState.BetweenWaves:
                text = String.Format(Loc.Get(LocalizationGroupId.GeneralInfoPanel, "wave_finished"), WaveModel.WaveIndex + 1);
                await ShowGeneralInfo(text, 800);
                break;
            case WaveState.AfterLastWave:
                text = Loc.Get(LocalizationGroupId.GeneralInfoPanel, "all_waves_finished");
                await ShowGeneralInfo(text, 800, showTimeMs: 1500);
                ShowWinPopup();
                break;
            case WaveState.Terminated:
                text = Loc.Get(LocalizationGroupId.GeneralInfoPanel, "defeat");
                await ShowGeneralInfo(text, 1000, Color.red);
                //show defeat popup
                break;
        }
    }

    private void OnWaveStateChanged()
    {
        UpdateGeneralInfo();
    }

    private async Task ShowGeneralInfo(string infoStr, int delayMs = 0, Color? textColor = null, int showTimeMs = 1000)
    {
        if (!_infoPanelCts.IsCancellationRequested)
        {
            _infoPanelCts.Cancel();
            _infoPanelCts.Dispose();
        }
        _infoPanelCts = new CancellationTokenSource();
        var stopToken = _infoPanelCts.Token;

        await DelayAsync(delayMs, stopToken);
        var infoPanelGo = Instantiate(UIPrefabsConfig.GeneralInfoPanelPrefab, UICanvasView.transform);
        var infoPanel = infoPanelGo.GetComponent<GeneralInfoPanelView>();

        if (textColor != null)
        {
            infoPanel.SetTextColor(textColor.Value);
        }
        await infoPanel.ShowAsync();
        if (!stopToken.IsCancellationRequested) await infoPanel.SetTextAsync(infoStr, stopToken);
        await Task.Delay(showTimeMs, stopToken).ContinueWith(_ => { });
        await infoPanel.HideAsync();

        Destroy(infoPanelGo);
    }

    private void ShowWinPopup()
    {
        var winPopupGo = Instantiate(UIPrefabsConfig.WinPopupPrefab, UICanvasView.transform);
    }

    private async Task DelayAsync(int delayMs, CancellationToken stopToken)
    {
        if (delayMs > 0 && !stopToken.IsCancellationRequested) await Task.Delay(delayMs, stopToken).ContinueWith(_ => { });
    }
}
