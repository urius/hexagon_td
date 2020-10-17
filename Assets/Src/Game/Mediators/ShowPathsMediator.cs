using System;
using System.Collections.Generic;
using UnityEngine;

public class ShowPathsMediator
{
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public ICellPositionConverter CellPositionConverter { get; set; }
    [Inject] public UIPrefabsConfig UIPrefabsConfig { get; set; }
    [Inject] public IUpdateProvider UpdateProvider { get; set; }
    [Inject] public IRootTransformProvider RootTransformProvider { get; set; }

    private readonly Vector3 _offset = Vector3.up;

    private GameObject _pathLinePrefab;
    private Material _lineMaterial;
    private Vector2 _lineTextureOffset;
    private GameObject _parentPrefab;
    private GameObject _parent;

    [PostConstruct]
    public void Construct()
    {
        _pathLinePrefab = UIPrefabsConfig.PathLinePrefab;
        _lineMaterial = _pathLinePrefab.GetComponent<LineRenderer>().sharedMaterial;

        _parentPrefab = new GameObject("[Display path container]");

        ShowPaths();
    }

    private void ShowPaths()
    {
        HidePaths();

        _lineTextureOffset = Vector2.zero;
        _parent = GameObject.Instantiate(_parentPrefab, RootTransformProvider.transform);
        var paths = LevelModel.GetPaths();
        foreach (var path in paths)
        {
            ShowPath(path);
        }

        UpdateProvider.UpdateAction += OnUpdate;
        LevelModel.PathsManager.PathsUpdated += OnPathsUpdated;
    }

    private void HidePaths()
    {
        UpdateProvider.UpdateAction -= OnUpdate;
        LevelModel.PathsManager.PathsUpdated -= OnPathsUpdated;

        if (_parent != null)
        {
            GameObject.Destroy(_parent);
            _parent = null;
        }
    }

    private void OnUpdate()
    {
        _lineTextureOffset.x -= Time.deltaTime * 2;
        _lineMaterial.SetTextureOffset("_MainTex", _lineTextureOffset);
    }

    private void OnPathsUpdated()
    {
        ShowPaths();
    }

    private void ShowPath(IReadOnlyList<Vector2Int> path)
    {
        for (var i = 0; i < path.Count - 1; i++)
        {
            var fromCell = path[i];
            var toCell = path[i + 1];
            if (IsNear(path[i], path[i + 1]))
            {
                var line = GameObject.Instantiate(_pathLinePrefab, _parent.transform).GetComponent<LineRenderer>();
                line.SetPositions(new Vector3[] {
                    CellPositionConverter.CellVec2ToWorld(fromCell) + _offset,
                    CellPositionConverter.CellVec2ToWorld(toCell) + _offset,
                });
            }
        }
    }

    private bool IsNear(Vector2Int cellA, Vector2Int cellB)
    {
        return Math.Abs(cellA.x - cellB.x) <= 1 && Math.Abs(cellA.y - cellB.y) <= 1;
    }
}
