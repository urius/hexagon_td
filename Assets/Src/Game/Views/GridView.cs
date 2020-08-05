using System;
using System.Collections.Generic;
using System.Linq;
using strange.extensions.mediation.impl;
using UnityEngine;

public interface ICellPositionConverter
{
    Vector2Int CellVec3ToVec2(Vector3Int cellPosition);
    Vector3 CellVec2ToWorld(Vector2Int cellPosition);
    Func<Vector3Int, Vector3> CellToWorld { get; }
    Func<Vector3, Vector3Int> WorldToCell { get; }
}

public class GridView : EventView, ICellPositionConverter
{
    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _plane;
    [SerializeField] private GameObject _staticObjectsParent;

    private bool _isTransposed;

    private Dictionary<Vector2Int, GameObject> _allCreatedCells = new Dictionary<Vector2Int, GameObject>();

    public void SetTransposed(bool isTransposed)
    {
        _isTransposed = isTransposed;

        transform.rotation = Quaternion.LookRotation(isTransposed ? Vector3.left : Vector3.forward, Vector3.up);
    }

    public void BakeStatic()
    {
        if (_staticObjectsParent != null)
        {
            StaticBatchingUtility.Combine(_staticObjectsParent);
        }
    }

    public void EraseCell(Vector2Int cellPosition)
    {
        if (_allCreatedCells.ContainsKey(cellPosition))
        {
            DestroyImmediate(_allCreatedCells[cellPosition]);
            _allCreatedCells.Remove(cellPosition);
        }
    }

    public GameObject DrawCell(Vector2Int cellPosition, GameObject prefab, bool isStatic = false)
    {
        var worldCellPosition = _grid.CellToWorld(CellVec2ToVec3(cellPosition));
        var cellParent = (isStatic && _staticObjectsParent != null) ? _staticObjectsParent.transform : _grid.transform;
        var go = Instantiate(prefab, worldCellPosition, _grid.transform.rotation, cellParent);
        _allCreatedCells[cellPosition] = go;
        return go;
    }

    public SpawnBaseCellView GetSpawnCellView(Vector2Int cellPosition)
    {
        return _allCreatedCells[cellPosition].GetComponent<SpawnBaseCellView>();
    }

    public TeleportCellView GetTeleportCellView(Vector2Int cellPosition)
    {
        return _allCreatedCells[cellPosition].GetComponent<TeleportCellView>();
    }

    public void DestroyAllCells()
    {
        foreach (var cellGo in _allCreatedCells.Values)
        {
            DestroyImmediate(cellGo);
        }

        _allCreatedCells.Clear();
    }

    public void UpdatePLaneSize()
    {
        var childCells = _grid.gameObject.GetComponentsInChildren<CellView>();
        if (childCells.Length == 0)
        {
            return;
        }
        var maxX = childCells.Max(c => c.transform.position.x);
        var minX = childCells.Min(c => c.transform.position.x);

        var maxY = childCells.Max(c => c.transform.position.z);
        var minY = childCells.Min(c => c.transform.position.z);

        var sizeHor = 0.1f * (maxX - minX);
        var sizeVer = 0.1f * (maxY - minY);

        _plane.transform.localScale = new Vector3(!_isTransposed ? sizeHor : sizeVer,
                                                    1,
                                                    !_isTransposed ? sizeVer : sizeHor);
        _plane.transform.position = new Vector3(
            (maxX + minX) / 2,
            _plane.transform.position.y,
            (maxY + minY) / 2);
    }

    public Vector3 CellVec2ToWorld(Vector2Int cellPosition)
    {
        return _grid.CellToWorld(CellVec2ToVec3(cellPosition));
    }

    public Vector2Int CellVec3ToVec2(Vector3Int cellPosition)
    {
        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

    public Func<Vector3Int, Vector3> CellToWorld => _grid.CellToWorld;
    public Func<Vector3, Vector3Int> WorldToCell => _grid.WorldToCell;
    public Vector3 CellSize => _grid.cellSize;
    public bool IsTransposed => _isTransposed;

    private static Vector3Int CellVec2ToVec3(Vector2Int vec2)
    {
        return new Vector3Int(vec2.x, vec2.y, 0);
    }
}