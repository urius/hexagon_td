using strange.extensions.dispatcher.eventdispatcher.api;

public class WavesControlSystem : EventSystemBase
{
    [Inject] public WaveModel WaveModel { get; set; }
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public LevelsCollectionProvider LevelsCollectionProvider { get; set; }
    [Inject] public LevelConfigProvider LevelConfigProvider { get; set; }

    public override void Start()
    {
        dispatcher.AddListener(CommandEvents.SECOND_PASSED, OnSecondPassed);
        dispatcher.AddListener(MediatorEvents.UI_START_WAVE_CLICKED, OnStartWaveClicked);
        dispatcher.AddListener(MediatorEvents.UI_LOSE_POPUP_CONTINUE_CLICKED, OnContinueWaveClicked);
    }

    private void OnStartWaveClicked(IEvent payload)
    {
        if (WaveModel.WaveState == WaveState.BeforeFirstWave || WaveModel.WaveState == WaveState.BetweenWaves)
        {
            WaveModel.AdvanceWave();
            WaveModel.StartWave();
            AudioManager.Instance.SetInWaveMusicMode(true);
        }
    }

    private void OnContinueWaveClicked(IEvent payload)
    {
        if (PlayerGlobalModelHolder.Model.TrySpendGold(LevelModel.GetContinueWavePrice()))
        {
            LevelModel.ContinueCurrentWave();
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
                AudioManager.Instance.SetInWaveMusicMode(false);
                if (WaveModel.WaveState == WaveState.AfterLastWave)
                {
                    UpdatePlayerData();

                    LevelModel.FinishLevel(true);
                }
                else
                {
                    LevelModel.TryAddMoney(LevelModel.WaveCompletionReward);
                    dispatcher.Dispatch(CommandEvents.UI_REQUEST_ANIMATE_WAVE_REWARD, LevelModel.WaveCompletionReward);
                }
            }
        }
    }

    private void UpdatePlayerData()
    {
        var levelIndex = LevelsCollectionProvider.GetLevelIndexByConfig(LevelConfigProvider.LevelConfig);
        var stars = LevelModel.GetAccuracyRate();
        PlayerGlobalModelHolder.Model.SetLevelPassed(levelIndex, stars);
    }
}
