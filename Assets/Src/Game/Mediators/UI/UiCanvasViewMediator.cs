using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using strange.extensions.dispatcher.eventdispatcher.api;
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
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_CLICKED, OnSettingsClicked);

        OnWaveStateChanged();
    }

    private void OnDestroy()
    {
        WaveModel.WaveStateChanged -= OnWaveStateChanged;
        dispatcher.RemoveListener(MediatorEvents.UI_SETTINGS_CLICKED, OnSettingsClicked);

        _infoPanelCts.Cancel();
        _infoPanelCts.Dispose();
    }

    private async void OnWaveStateChanged()
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
                await DelayAsync(800, CancellationToken.None);
                text = String.Format(Loc.Get(LocalizationGroupId.GeneralInfoPanel, "wave_finished"), WaveModel.WaveIndex + 1);
                AudioManager.Instance.Play(SoundId.WaveFinished);
                await ShowGeneralInfo(text);
                break;
            case WaveState.AfterLastWave:
                await AudioManager.Instance.FadeOutAndStopMusicAsync();
                AudioManager.Instance.Play(SoundId.LevelComplete);
                text = Loc.Get(LocalizationGroupId.GeneralInfoPanel, "all_waves_finished");
                await ShowGeneralInfo(text, 800, showTimeMs: 1500);
                ShowPopup(UIPrefabsConfig.WinPopupPrefab);
                break;
            case WaveState.Terminated:
                await AudioManager.Instance.FadeOutAndStopMusicAsync();
                AudioManager.Instance.Play(SoundId.LevelLose);
                text = Loc.Get(LocalizationGroupId.GeneralInfoPanel, "defeat");
                ShowPopup(UIPrefabsConfig.LosePopupPrefab);
                var genInfoTask = ShowGeneralInfo(text, textColor: Color.red, showTimeMs: 1500);
                break;
        }
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

    private void OnSettingsClicked(IEvent payload)
    {
        ShowPopup(UIPrefabsConfig.SettingsPopupPrefab);
    }

    private void ShowPopup(GameObject popupPrefab)
    {
        Instantiate(popupPrefab, UICanvasView.transform);
    }

    private async Task DelayAsync(int delayMs, CancellationToken stopToken)
    {
        if (delayMs > 0 && !stopToken.IsCancellationRequested) await Task.Delay(delayMs, stopToken).ContinueWith(_ => { });
    }
}
