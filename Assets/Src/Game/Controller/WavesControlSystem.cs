using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class WavesControlSystem : EventSystemBase
{
    [Inject] public WaveModel WaveModel { get; set; }
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public PlayerGlobalModelHolder PlayerGlobalModelHolder { get; set; }
    [Inject] public LevelsCollectionProvider LevelsCollectionProvider { get; set; }
    [Inject] public LevelConfigProvider LevelConfigProvider { get; set; }

    public override void Start()
    {
        dispatcher.AddListener(CommandEvents.SECOND_PASSED, OnSecondPassed);
        dispatcher.AddListener(MediatorEvents.UI_START_WAVE_CLICKED, OnStartWaveClicked);
    }

    private void OnStartWaveClicked(IEvent payload)
    {
        if (WaveModel.WaveState == WaveState.BeforeFirstWave || WaveModel.WaveState == WaveState.BetweenWaves)
        {
            if (WaveModel.WaveState == WaveState.BetweenWaves)
            {
                WaveModel.AdvanceWave();
            }
            WaveModel.StartWave();
        }
    }

    private void OnSecondPassed(IEvent payload)
    {
        if (WaveModel.WaveState == WaveState.InWave)
        {
            if (WaveModel.IsCurrentWaveEmpty && LevelUnitsModel.UnitsCount == 0)
            {
                LevelModel.SetTimeScale(1);

                WaveModel.EndWave();
                if (WaveModel.WaveState == WaveState.AfterLastWave)
                {
                    LevelModel.FinishLevel(true);

                    UpdatePlayerData();
                } else
                {
                    LevelModel.TryAddMoney(LevelModel.WaveCompletionReward);
                }
            }
        }
    }

    private void UpdatePlayerData()
    {
        var levelIndex = LevelsCollectionProvider.GetLevelIndexByConfig(LevelConfigProvider.LevelConfig);
        var stars = LevelModel.GetAccuracyRate();
        PlayerGlobalModelHolder.PlayerGlobalModel.SetLevelPassed(levelIndex, stars);
    }
}
