using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using UnityEngine;

public class TimeScaleChangeCommand : EventCommand
{
    public override void Execute()
    {
        if (Time.timeScale >= 3)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale++;
        }
    }
}
