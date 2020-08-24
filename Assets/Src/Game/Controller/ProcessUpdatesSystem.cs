using System;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class ProcessUpdatesSystem : EventSystemBase
{
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }
    [Inject] public UnitConfigsProvider UnitConfigsProvider { get; set; }
    [Inject] public IUpdateProvider UpdateProvider { get; set; }

    private int _framesCount = 0;

    public ProcessUpdatesSystem()
    {
    }

    public override void Start()
    {
        UpdateProvider.UpdateAction += OnUpdate;
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
