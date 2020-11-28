using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using UnityEngine;

public class TimeScaleChangeCommand : EventCommand
{
    [Inject] public LevelModel LevelModel { get; set; }

    public override void Execute()
    {
        if (LevelModel.TimeScale >= 3)
        {
            LevelModel.SetTimeScale(1);
        }
        else
        {
            LevelModel.SetTimeScale(LevelModel.TimeScale + 1);
        }
    }
}
