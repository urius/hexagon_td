using System;
using strange.extensions.mediation.impl;

public class WaveTextViewMediator : EventMediator
{
    [Inject] public WaveTextView WaveTextView { get; set; }
    [Inject] public WaveModel WaveModel { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        WaveModel.WaveStateChanged += OnWaveStateChanged;
        UpdateView();
    }

    private void OnWaveStateChanged()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        if (WaveModel.WaveState == WaveState.InWave)
        {
            WaveTextView.SetActive(true);
            WaveTextView.SetText(
                $"{Loc.Get(LocalizationGroupId.BottomPanel, "wave")}: {WaveModel.WaveIndex + 1}/{WaveModel.TotalWavesCount}");
        }
        else if (WaveModel.WaveState != WaveState.AfterLastWave)
        {
            WaveTextView.SetActive(false);
        }
    }
}
