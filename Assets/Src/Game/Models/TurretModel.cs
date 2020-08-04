using System;
using UnityEngine;

public class TurretModel
{
    public event Action NewTargetSet = delegate { };
    public event Action Fired = delegate { };

    public readonly TurretType TurretType;
    public readonly Vector2Int Position;
    public readonly int AttackRadiusCells;
    public readonly int Damage;
    public readonly int ReloadTimeFrames;
    public readonly TurretConfig TurretConfig;

    public TurretModel(TurretConfig turretConfig, Vector2Int position)
    {
        TurretConfig = turretConfig;

        TurretType = turretConfig.TurretType;
        AttackRadiusCells = turretConfig.AttackRadiusCells;
        Damage = turretConfig.Damage;
        ReloadTimeFrames = turretConfig.ReloadTimeFrames;
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

    private void OnTargetUnitStateUpdated()
    {
        if (TargetUnit.CurrentState.StateName == UnitStateName.Destroing)
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
        ReloadFramesLeft = ReloadTimeFrames;
        Fired();
    }
}
