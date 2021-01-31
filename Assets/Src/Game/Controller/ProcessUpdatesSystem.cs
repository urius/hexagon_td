using System;
using UnityEngine;

public class ProcessUpdatesSystem : EventSystemBase
{
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }
    [Inject] public UnitConfigsProvider UnitConfigsProvider { get; set; }
    [Inject] public PlayerGlobalModelHolder PlayerGlobalModelHolder { get; set; }
    [Inject] public IUpdateProvider UpdateProvider { get; set; }

    private int _framesCount = 0;

    public ProcessUpdatesSystem()
    {
    }

    public override void Start()
    {
        LevelModel.LevelFinished += OnLevelFinished;

        UpdateProvider.UpdateAction += OnUpdate;
    }

    private async void OnLevelFinished()
    {
        LevelModel.SetTimeScale(1);

        UpdateProvider.UpdateAction -= OnUpdate;

        await NetworkManager.SaveUserDataAsync(PlayerGlobalModelHolder.PlayerGlobalModel);
    }

    private void OnUpdate()
    {
        LevelModel.Update();

        ProcessTurrets();

        _framesCount++;
        if (_framesCount >= 60)
        {
            _framesCount = 0;
            dispatcher.Dispatch(CommandEvents.SECOND_PASSED);
        }
    }

    private void ProcessTurrets()
    {
        foreach (var turret in LevelModel.LevelTurretsModel.TurretsList)
        {
            if (turret.CanFire)
            {
                turret.Fire();
            }
        }
    }
}
