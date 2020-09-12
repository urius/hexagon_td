using strange.extensions.dispatcher.eventdispatcher.api;

public class TurretsControlSystem : EventSystemBase
{
    [Inject] public TurretConfigProvider TurretConfigProvider { get; set; }
    [Inject] public LevelTurretsModel LevelTurretsModel { get; set; }
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }

    public override void Start()
    {
        dispatcher.AddListener(MediatorEvents.TURRET_DETECTED_UNIT_IN_ATTACK_ZONE, OnUnitDetectedInZone);
        dispatcher.AddListener(MediatorEvents.TURRET_TARGET_LOCKED, OnTargetLocked);
        dispatcher.AddListener(MediatorEvents.TURRET_UNIT_LEAVE_ATTACK_ZONE, OnUnitLeaveZone);
        dispatcher.AddListener(MediatorEvents.TURRET_SELL_CLICKED, OnTurretSellClicked);
        dispatcher.AddListener(MediatorEvents.TURRET_UPGRADE_CLICKED, OnTurretUpgradeClicked);
    }

    private void OnUnitDetectedInZone(IEvent payload)
    {
        var data = (MediatorEventsParams.TurretUnitInAttackZoneParams)payload.data;

        data.TurretModel.SetTarget(data.UnitModel);
        if (data.TurretModel.TurretType == TurretType.SlowField)
        {
            data.UnitModel.AffectBySlowTurret(data.TurretModel);
        }
    }

    private void OnTargetLocked(IEvent payload)
    {
        (payload.data as TurretModel).LockTarget();
    }

    private void OnUnitLeaveZone(IEvent payload)
    {
        var data = (MediatorEventsParams.TurretUnitLeaveAttackZoneParams)payload.data;

        if (data.TurretModel.TurretType == TurretType.SlowField)
        {
            data.UnitModel.RemoveSlowTurretAffect(data.TurretModel);
        }

        data.TurretModel.SetTarget(null);
    }

    private void OnTurretUpgradeClicked(IEvent payload)
    {
        var turret = payload.data as TurretModel;

        var upgradedTurretConfig = TurretConfigProvider.GetConfig(turret.TurretType, turret.TurretConfig.TurretLevelIndex + 1);
        if (upgradedTurretConfig != null)
        {
            turret.Upgrade(upgradedTurretConfig);
        }
    }

    private void OnTurretSellClicked(IEvent payload)
    {
        var turret = payload.data as TurretModel;

        turret.Destroy();
        LevelTurretsModel.RemoveTurret(turret);
        foreach (var unitModel in LevelUnitsModel.Units)
        {
            unitModel.RemoveSlowTurretAffect(turret);
        }

        //TODO: add money
    }
}
