using System;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class ProcessUpdatesCommand
{
    [Inject(ContextKeys.CONTEXT_DISPATCHER)]
    public IEventDispatcher dispatcher { get; set; }

    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }
    [Inject] public UnitConfigsProvider UnitConfigsProvider { get; set; }
    [Inject] public IUpdateProvider UpdateProvider { get; set; }

    private int _framesCount = 0;

    public ProcessUpdatesCommand()
    {
    }

    public void Start()
    {
        UpdateProvider.UpdateAction += OnUpdate;
    }

    private void OnUpdate()
    {
        LevelModel.Update();

        ProcessTurrets();

        if (_framesCount % 20 == 0)
        {
            foreach (var waitingUnit in LevelUnitsModel.WaitingUnits)
            {
                if (LevelUnitsModel.IsCellWithoutUnit(waitingUnit.NextCellPosition))
                {
                    dispatcher.Dispatch(CommandEvents.UPDATE_UNIT_STATE, waitingUnit);
                }
            }

            Spawn();
        }

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

    private void Spawn()
    {
        var waveModel = LevelModel.WaveModel;
        foreach (var spawnCell in LevelModel.SpawnCells)
        {
            if (!waveModel.IsCurrentWaveEmpty)
            {
                if (LevelUnitsModel.IsCellWithoutUnit(spawnCell.CellPosition))
                {
                    var unitType = waveModel.GetUnitAndIncrement();
                    var unitConfig = UnitConfigsProvider.GetConfigByType(unitType);

                    //injectionBinder.Unbind<UnitModel>();
                    //injectionBinder.Bind<UnitModel>().ToValue(unitModel);
                    //  OnRegister happens on next frame, so this workaround is not working

                    var path = LevelModel.GetPath(spawnCell.CellPosition);
                    var unitModel = new UnitModel(path, unitConfig);
                    LevelUnitsModel.AddUnit(unitModel);
                    LevelUnitsModel.OwnCellByUnit(unitModel);

                    dispatcher.Dispatch(CommandEvents.START_SPAWN_UNIT, unitModel);
                }
            }
        }
    }
}
