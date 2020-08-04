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
    }
}
