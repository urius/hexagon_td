﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RequestBuildTurretCommand : ParamCommand<MediatorEventsParams.RequestBuildParams>
{
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public TurretConfigProvider TurretConfigProvider { get; set; }

    public RequestBuildTurretCommand()
    {
    }

    public override void Execute(MediatorEventsParams.RequestBuildParams data)
    {
        if (LevelModel.IsReadyToBuild(data.GridPosition))
        {
            var turretConfig = TurretConfigProvider.GetConfig(data.TurretType, 0);
            var turretModel = new TurretModel(turretConfig, data.GridPosition);
            LevelModel.LevelTurretsModel.AddTurret(turretModel);

            dispatcher.Dispatch(CommandEvents.BUILD_TURRET, turretModel);
        }
    }
}
