using System;
using System.Collections;
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

        State = UnitStateName.Spawning;
    }

    public GameObject Prefab => _config.Prefab;
    public Vector2Int PreviousCellPosition => _path[ClapmCellIndex(_currentPathCellIndex - 1)];
    public Vector2Int CurrentCellPosition => _path[_currentPathCellIndex];
    public Vector2Int NextCellPosition => _path[ClapmCellIndex(_currentPathCellIndex + 1)];
    public UnitStateName State { get; private set; }


    public void AdvanceState()
    {
        State = GetState();

        _currentPathCellIndex = ClapmCellIndex(_currentPathCellIndex + 1);

        StateUpdated();
    }

    private UnitStateName GetState()
    {
        if (_currentPathCellIndex >= _path.Count - 1)
        {
            return UnitStateName.Destroing;
        }
        return UnitStateName.Moving;
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

public enum UnitStateName
{
    Spawning,
    Moving,
    Destroing,
}
