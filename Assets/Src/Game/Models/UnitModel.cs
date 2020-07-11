using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitModel
{
    public event Action StateUpdated = delegate { };

    private int _currentPathCellIndex;

    private readonly IReadOnlyList<Vector2Int> _path;
    private readonly UnitConfig _config;

    public UnitModel(IReadOnlyList<Vector2Int> path, UnitConfig config)
    {
        _path = path;
        _config = config;

        CurrentState = new SpawningState(CurrentCellPosition);
    }

    public GameObject Prefab => _config.Prefab;
    public Vector2Int PreviousCellPosition => _path[ClapmCellIndex(_currentPathCellIndex - 1)];
    public Vector2Int CurrentCellPosition => _path[_currentPathCellIndex];
    public Vector2Int NextCellPosition => _path[ClapmCellIndex(_currentPathCellIndex + 1)];

    public UnitStateBase PreviousState { get; private set; }
    public UnitStateName PreviousStateName => PreviousState.StateName;
    public UnitStateBase CurrentState { get; private set; }
    public UnitStateName CurrentStateName => CurrentState.StateName;
    public bool IsOnLastCell => _currentPathCellIndex >= _path.Count - 1;
    public bool IsNextCellNear => !IsCellsNotNear(NextCellPosition, CurrentCellPosition);

    public void IncrementCellIndex()
    {
        _currentPathCellIndex++;
    }

    public void SetState(UnitStateBase state)
    {
        PreviousState = CurrentState;
        CurrentState = state;

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

    private int ClapmCellIndex(int index)
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

    public MovingState(Vector2Int targetPosition, Vector2Int fromPosition)
        : base(targetPosition)
    {
        FromPosition = fromPosition;
        IsTeleporting = UnitModel.IsCellsNotNear(targetPosition, fromPosition);
    }

    public override UnitStateName StateName => UnitStateName.Moving;
}

public class DestroingState : UnitStateBase
{
    public DestroingState(Vector2Int position)
        : base(position)
    {
    }

    public override UnitStateName StateName => UnitStateName.Destroing;
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
    Destroing,
}

#endregion