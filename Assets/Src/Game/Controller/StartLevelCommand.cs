using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using UnityEngine;

public class StartLevelCommand : Command
{
    [Inject] public LevelModel levelModel { get; set; }

    public override void Execute()
    {
        levelModel.IsInitialized = true;
    }
}
