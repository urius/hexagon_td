using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using UnityEngine;

public class SecondPassedCommand : Command
{
    [Inject] public LevelModel levelModel { get; set; }
    [Inject] public UnitConfigsProvider unitConfigsProvider { get; set; }


    public override void Execute()
    {
        Debug.Log("SecondPassedCommand");

        if (!levelModel.IsInitialized)
        {
            return;
        }
        Spawn();
    }

    private void Spawn()
    {
        var waveModel = levelModel.WaveModel;
        foreach (var spawnCell in levelModel.SpawnCells)
        {
            if (!waveModel.IsCurrentWaveEmpty)
            {
                if (levelModel.IsCellFree(spawnCell.CellPosition))
                {
                    var unitType = waveModel.GetUnitAndIncrement();
                    var unitConfig = unitConfigsProvider.GetConfigByType(unitType);

                    //injectionBinder.Unbind<UnitModel>();
                    //injectionBinder.Bind<UnitModel>().ToValue(unitModel);
                    //  OnRegister happens on next frame, so this workaround is not working

                    levelModel.SpawnUnit(spawnCell.CellPosition, unitConfig);
                }
            }
        }
    }
}
