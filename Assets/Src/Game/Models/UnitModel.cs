using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitModel
{
    public event Action StateUpdated = delegate { };
    public event Action HpChanged = delegate { };

    private int _currentPathCellIndex = 0;
    private IReadOnlyList<Vector2Int> _path;
    private float _turretsSpeedMultiplier = 1;

    private readonly UnitConfig _config;
    private readonly LinkedList<TurretModel> _slowDownAffectors = new LinkedList<TurretModel>();

    public UnitModel(IReadOnlyList<Vector2Int> path, UnitConfig config)
    {
        _path = path;
        _config = config;

        PreviousCellPosition = CurrentCellPosition = _path[0];
        NextCellPosition = _path[1];

        HP = config.HP;
        Speed = config.Speed;

        CurrentState = new SpawningState(CurrentCellPosition);
    }

    public GameObject Prefab => _config.Prefab;
    public int HP { get; private set; }
    public float Speed { get; private set; }

    public Vector2Int PreviousCellPosition { get; private set; }
    public Vector2Int CurrentCellPosition { get; private set; }
    public Vector2Int NextCellPosition { get; private set; }

    public UnitStateBase PreviousState { get; private set; }
    public UnitStateName PreviousStateName => PreviousState.StateName;
    public UnitStateBase CurrentState { get; private set; }

    public UnitStateName CurrentStateName => CurrentState.StateName;
    public bool IsOnLastCell => _currentPathCellIndex >= _path.Count - 1;
    public bool IsOnPreLastCell => _currentPathCellIndex == _path.Count - 2;
    public bool IsNextCellNear => !IsCellsNotNear(NextCellPosition, CurrentCellPosition);
    public bool IsDestroying => CurrentState.StateName == UnitStateName.Destroying;

    public void SetState(UnitStateBase state)
    {
        PreviousState = CurrentState;
        CurrentState = state;
        if (state.StateName == UnitStateName.Moving)
        {
            _currentPathCellIndex++;
            Speed = (state as MovingState).SpeedMultiplier * _turretsSpeedMultiplier * _config.Speed;
            PreviousCellPosition = CurrentCellPosition;
            CurrentCellPosition = NextCellPosition;
            NextCellPosition = _path[ClampCellIndex(_currentPathCellIndex + 1)];
        }

        StateUpdated();
    }

    public static bool IsCellsNotNear(Vector2Int nextCellPosition, Vector2Int currentCellPosition)
    {
        if (Math.Abs(nextCellPosition.x - currentCellPosition.x) > 1
            || Math.Abs(nextCellPosition.y - currentCellPosition.y) > 1)
        {
            return true;
        }
        return false;
    }

    public (Vector2Int, Vector2Int) GetRestPathEdgePoints()
    {
        return (CurrentCellPosition, _path[_path.Count - 1]);
    }

    public bool IsRestPathContainsCell(Vector2Int cell)
    {
        for (var i = _currentPathCellIndex; i < _path.Count; i++)
        {
            if (_path[i] == cell)
            {
                return true;
            }
        }

        return false;
    }

    public void SubstitutePath(Vector2Int[] newPath)
    {
        _path = newPath;
        _currentPathCellIndex = 0;
        NextCellPosition = _path[1];
    }

    public int ApplyDamage(int bulletDamage)
    {
        HP = Math.Max(HP - bulletDamage, 0);
        HpChanged();
        return HP;
    }

    public void Repair(int hpAmount)
    {
        HP = Math.Min(HP + hpAmount, _config.HP);
        HpChanged();
    }

    public void SetDestroingState()
    {
        SetState(new DestroingState(CurrentCellPosition));

        while (_slowDownAffectors.Count > 0)
        {
            RemoveSlowTurretAffect(_slowDownAffectors.First.Value);
        }
    }

    public void AffectBySlowTurret(TurretModel turretModel)
    {
        turretModel.Upgraded += OnTurretUpgraded;

        _slowDownAffectors.AddLast(turretModel);

        RecalculateTurretsSlowDownModifier();
    }

    public void RemoveSlowTurretAffect(TurretModel turretModel)
    {
        turretModel.Upgraded -= OnTurretUpgraded;

        _slowDownAffectors.Remove(turretModel);

        RecalculateTurretsSlowDownModifier();
    }

    private void OnTurretUpgraded()
    {
        RecalculateTurretsSlowDownModifier();
    }

    private void RecalculateTurretsSlowDownModifier()
    {
        if (_slowDownAffectors.Any())
        {
            _turretsSpeedMultiplier = _slowDownAffectors.Min(t => t.SpeedMultiplier);
        }
        else
        {
            _turretsSpeedMultiplier = 1;
        }
    }

    private int ClampCellIndex(int index)
    {
        if (index <= 0)
        {
            return 0;
        }
        if (index > _path.Count - 1)
        {
            return _path.Count - 1;
        }

        return index;
    }
}

# region States
public abstract class UnitStateBase
{
    public readonly Vector2Int Position;

    public UnitStateBase(Vector2Int position)
    {
        Position = position;
    }

    public abstract UnitStateName StateName { get; }
}

public class SpawningState : UnitStateBase
{
    public SpawningState(Vector2Int spawnPosition)
        : base(spawnPosition)
    {
    }

    public override UnitStateName StateName => UnitStateName.Spawning;
}

public class MovingState : UnitStateBase
{
    public readonly Vector2Int FromPosition;
    public readonly bool IsTeleporting;
    public readonly float SpeedMultiplier;

    public MovingState(Vector2Int targetPosition, Vector2Int fromPosition, float speedMultiplier = 1)
        : base(targetPosition)
    {
        FromPosition = fromPosition;
        IsTeleporting = UnitModel.IsCellsNotNear(targetPosition, fromPosition);
        SpeedMultiplier = speedMultiplier;
    }

    public override UnitStateName StateName => UnitStateName.Moving;
}

public class DestroingState : UnitStateBase
{
    public DestroingState(Vector2Int position)
        : base(position)
    {
    }

    public override UnitStateName StateName => UnitStateName.Destroying;
}

public class WaitingState : UnitStateBase
{
    public readonly Vector2Int WaitPosition;
    public WaitingState(Vector2Int position, Vector2Int waitPosition)
        : base(position)
    {
        WaitPosition = waitPosition;
    }

    public override UnitStateName StateName => UnitStateName.WaitingForCell;
}

public enum UnitStateName
{
    Undefined,
    Spawning,
    Moving,
    WaitingForCell,
    Destroying,
}

#endregion