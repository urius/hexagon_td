using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using UnityEngine;

public class SecondPartPassedCommand : EventCommand
{
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public UnitConfigsProvider UnitConfigsProvider { get; set; }

    public override void Execute()
    {
        foreach (var waitingUnit in LevelModel.WaitingUnits)
        {
            if (LevelModel.IsCellFree(waitingUnit.NextCellPosition))
            {
                dispatcher.Dispatch(CommandEvents.UPDATE_UNIT_STATE, waitingUnit);
            }
        }

        Spawn();
    }

    private void Spawn()
    {
        var waveModel = LevelModel.WaveModel;
        foreach (var spawnCell in LevelModel.SpawnCells)
        {
            if (!waveModel.IsCurrentWaveEmpty)
            {
                if (LevelModel.IsCellFree(spawnCell.CellPosition))
                {
                    var unitType = waveModel.GetUnitAndIncrement();
                    var unitConfig = UnitConfigsProvider.GetConfigByType(unitType);

                    //injectionBinder.Unbind<UnitModel>();
                    //injectionBinder.Bind<UnitModel>().ToValue(unitModel);
                    //  OnRegister happens on next frame, so this workaround is not working

                    LevelModel.SpawnUnit(spawnCell.CellPosition, unitConfig);
                }
            }
        }
    }
}
