using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugShowPath : MonoBehaviour
{
    [SerializeField] private LevelConfig _levelConfig;
    [SerializeField] private Grid _grid;
    private Transform _startPointPrev;
    [SerializeField] private Transform _startPoint;
    private Transform _endPointPrev;
    [SerializeField] private Transform _endPoint;

    private Vector2Int[] _path;

    void Start()
    {
    }

    void Update()
    {
        if (_startPoint == null || _endPoint == null)
        {
            return;
        }

        if (_startPointPrev != _startPoint || _endPointPrev != _endPoint)
        {
            _startPointPrev = _startPoint;
            _endPointPrev = _endPoint;

            CalculatePath();
        }
    }

    private void CalculatePath()
    {
        var cellsProvider = new LevelPathfinderCellsProvider(_levelConfig);
        var start = _grid.WorldToCell(_startPoint.position);
        var end = _grid.WorldToCell(_endPoint.position);

        _path = Pathfinder.FindPath(cellsProvider, new Vector2Int(start.x, start.y), new Vector2Int(end.x, end.y));
    }

    private void OnDrawGizmos()
    {
        if (_path != null)
        {
            foreach (var point in _path)
            {
                Gizmos.DrawSphere(_grid.CellToWorld(new Vector3Int(point.x, point.y, 0)), 1);
            }
        }
    }
}
