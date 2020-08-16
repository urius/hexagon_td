﻿using System;
using UnityEngine;

public class GameScreenClickedCommand : ParamCommand<Vector2Int>
{
    [Inject] public LevelTurretsModel LevelTurretsModel { get; set; }

    public GameScreenClickedCommand()
    {

    }

    public override void Execute(Vector2Int cellPosition)
    {
        Debug.Log("GameScreenClickedCommand");

        if (LevelTurretsModel.TryGetTurret(cellPosition, out var turretModel))
        {
            injectionBinder.GetInstance<TurretActionsMediator>()
                .Initialize(turretModel);
        }
    }
}
