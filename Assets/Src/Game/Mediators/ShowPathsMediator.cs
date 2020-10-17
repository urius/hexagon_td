using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRuby.Tween;
using strange.extensions.mediation.impl;
using UnityEngine;

public class ShowPathsMediator : EventMediator
{
    private const string PathsAlphaTweenKey = "PathsAlphaTweenKey";
    private const float PathAlpha = 0.6f;

    [Inject] public ShowPathsView View { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public WaveModel WaveModel { get; set; }
    [Inject] public ICellPositionConverter CellPositionConverter { get; set; }
    [Inject] public UIPrefabsConfig UIPrefabsConfig { get; set; }
    [Inject] public IUpdateProvider UpdateProvider { get; set; }

    private readonly Vector3 _offset = Vector3.up;

    private GameObject _pathLinePrefab;
    private Material _lineMaterial;
    private Vector2 _lineTextureOffset;
    private List<LineRenderer> _lines = new List<LineRenderer>();

    public override void OnRegister()
    {
        base.OnRegister();

        _pathLinePrefab = UIPrefabsConfig.PathLinePrefab;
        _lineMaterial = _pathLinePrefab.GetComponent<LineRenderer>().sharedMaterial;

        WaveModel.WaveStateChanged += OnWaveStateChanged;

        ShowPaths();
    }

    private void OnDestroy()
    {
        WaveModel.WaveStateChanged -= OnWaveStateChanged;

        UpdateProvider.UpdateAction -= OnUpdate;
        LevelModel.PathsManager.PathsUpdated -= OnPathsUpdated;
    }

    private void OnWaveStateChanged()
    {
        switch (WaveModel.WaveState)
        {
            case WaveState.AfterLastWave:
            case WaveState.Terminated:
                break;
            case WaveState.BetweenWaves:
            case WaveState.BeforeFirstWave:
                ShowPaths();
                break;
            case WaveState.InWave:
                HidePaths();
                break;
        }
    }

    private void ShowPaths()
    {
        RemovePaths();

        _lineTextureOffset = Vector2.zero;
        
        var paths = LevelModel.GetPaths();
        _lines.Capacity = paths.Sum(p => p.Count);
        foreach (var path in paths)
        {
            CreatePath(path);
        }

        UpdateProvider.UpdateAction += OnUpdate;
        LevelModel.PathsManager.PathsUpdated += OnPathsUpdated;

        TweenFactory.Tween(PathsAlphaTweenKey, 0f, PathAlpha, 0.8f, TweenScaleFunctions.Linear, t => SetLinesAlpha(t.CurrentValue));
    }

    private void HidePaths()
    {
        TweenFactory.Tween(PathsAlphaTweenKey, PathAlpha, 0f, 0.8f, TweenScaleFunctions.Linear, t => SetLinesAlpha(t.CurrentValue), t => RemovePaths());
    }

    private void SetLinesAlpha(float alpha)
    {
        foreach (var line in _lines)
        {
            var color1 = line.startColor;
            var color2 = line.endColor;
            color1.a = color2.a = alpha;
            line.startColor = color1;
            line.endColor = color2;
        }
    }

    private void RemovePaths()
    {
        TweenFactory.RemoveTweenKey(PathsAlphaTweenKey, TweenStopBehavior.DoNotModify);

        UpdateProvider.UpdateAction -= OnUpdate;
        LevelModel.PathsManager.PathsUpdated -= OnPathsUpdated;

        foreach (var line in _lines)
        {
            GameObject.Destroy(line.gameObject);
        }
        _lines.Clear();
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

    private void CreatePath(IReadOnlyList<Vector2Int> path)
    {
        for (var i = 0; i < path.Count - 1; i++)
        {
            var fromCell = path[i];
            var toCell = path[i + 1];
            if (IsNear(path[i], path[i + 1]))
            {
                var line = GameObject.Instantiate(_pathLinePrefab, View.transform).GetComponent<LineRenderer>();
                line.SetPositions(new Vector3[] {
                    CellPositionConverter.CellVec2ToWorld(fromCell) + _offset,
                    CellPositionConverter.CellVec2ToWorld(toCell) + _offset,
                });
                _lines.Add(line);
            }
        }
    }

    private bool IsNear(Vector2Int cellA, Vector2Int cellB)
    {
        return Math.Abs(cellA.x - cellB.x) <= 1 && Math.Abs(cellA.y - cellB.y) <= 1;
    }
}
