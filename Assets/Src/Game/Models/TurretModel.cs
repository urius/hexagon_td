using System;
using UnityEngine;

public class TurretModel
{
    public event Action NewTargetSet = delegate { };
    public event Action Fired = delegate { };
    public event Action Upgraded = delegate { };
    public event Action Destroyed = delegate { };
    public event Action<int> SellAnimationTriggered = delegate { };

    public readonly Vector2Int Position;

    public TurretType TurretType;
    public int AttackRadiusCells;
    public int Damage;
    public float SpeedMultiplier;
    public int ReloadTimeFrames;
    public TurretConfig TurretConfig;

    public bool IsDestroyed = false;

    public TurretModel(TurretConfig turretConfig, Vector2Int position)
    {
        SetupConfig(turretConfig);

        ReloadFramesLeft = 0;

        Position = position;
    }

    public int ReloadFramesLeft { get; private set; }
    public UnitModel TargetUnit { get; private set; }
    public bool CanFire => ReloadFramesLeft <= 0 && TargetUnit != null && TargetLocked;
    public bool TargetLocked { get; private set; }

    public void SetTarget(UnitModel unitModel)
    {
        if (TargetUnit != null)
        {
            TargetUnit.StateUpdated -= OnTargetUnitStateUpdated;
        }

        TargetUnit = unitModel;
        if (TargetUnit == null)
        {
            TargetLocked = false;
        }
        else
        {
            TargetUnit.StateUpdated += OnTargetUnitStateUpdated;
        }
        NewTargetSet();
    }

    public void Upgrade(TurretConfig turretConfig)
    {
        SetupConfig(turretConfig);
        Upgraded();
    }

    private void SetupConfig(TurretConfig turretConfig)
    {
        TurretConfig = turretConfig;

        TurretType = turretConfig.TurretType;
        AttackRadiusCells = turretConfig.AttackRadiusCells;
        Damage = turretConfig.Damage;
        SpeedMultiplier = turretConfig.SpeedMultiplier;
        ReloadTimeFrames = turretConfig.ReloadTimeFrames;
    }

    private void OnTargetUnitStateUpdated()
    {
        if (TargetUnit.CurrentState.StateName == UnitStateName.Destroying)
        {
            SetTarget(null);
        }
    }

    public void LockTarget()
    {
        TargetLocked = true;
    }

    public void Update()
    {
        if (ReloadFramesLeft > 0)
        {
            ReloadFramesLeft--;
        }
    }

    public void Fire()
    {
        if (IsDestroyed) return;
        ReloadFramesLeft = ReloadTimeFrames;
        Fired();
    }

    internal void TriggerSellAnimation(int sellPrice)
    {
        SellAnimationTriggered(sellPrice);
    }

    public void Destroy()
    {
        IsDestroyed = true;
        SetTarget(null);

        Destroyed();
    }
}
