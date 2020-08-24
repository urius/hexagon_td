﻿using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class BulletsHitSystem : EventSystemBase
{
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }

    public BulletsHitSystem()
    {
    }

    public override void Start()
    {
        dispatcher.AddListener(MediatorEvents.BULLET_HIT_TARGETS, OnBulletHitTarget);
    }

    private void OnBulletHitTarget(IEvent payload)
    {
        var param = payload.data as MediatorEventsParams.BulletHitTargetsParams;
        foreach (var unit in param.UnitModels)
        {
            if (unit.HP > 0 && unit.ApplyDamage(param.BulletDamage) <= 0)
            {
                LevelUnitsModel.DestroyUnit(unit);
            }
        }
    }
}
