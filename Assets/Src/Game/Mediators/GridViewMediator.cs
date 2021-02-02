using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class GridViewMediator : EventMediator
{
    private GameObject _flyingTurret;
    private TurretType _flyingTurretType;

    [Inject] public GridView gridView { get; set; }
    [Inject] public LevelModel levelModel { get; set; }
    [Inject] public LevelUnitsModel unitsModel { get; set; }
    [Inject] public CellConfigProvider cellConfigProvider { get; set; }
    [Inject] public ModelByViewHolder modelByView { get; set; }
    [Inject] public ICellPositionConverter cellPositionConverter { get; set; }
    [Inject] public GridViewProvider gridViewProvider { get; set; }
    [Inject] public ICellSizeProvider cellSizeProvider { get; set; }
    [Inject] public IRootTransformProvider rootTransformProvider { get; set; }
    [Inject] public TurretConfigProvider turretsConfigProvider { get; set; }
    [Inject] public WorldMousePositionProvider worldMousePositionProvider { get; set; }
    [Inject] public FlyingTurretConfigProvider FlyingTurretConfigProvider { get; set; }

    public override void OnRegister()
    {
        dispatcher.AddListener(CommandEvents.START_SPAWN_UNIT, OnStartSpawnUnit);
        dispatcher.AddListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_DOWN, OnBuildTurretMouseDown);
        dispatcher.AddListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_UP, OnBuildTurretMouseUp);
        dispatcher.AddListener(MediatorEvents.UNIT_DESTROY_ANIMATION_FINISHED, OnUnitDestroyAnimationFinished);
        unitsModel.UnitRemoved += OnUnitRemoved;
        levelModel.Teleporting += OnTeleporting;
        levelModel.GoalCountUpdated += OnGoalCountUpdated;

        gridViewProvider.SetGridView(gridView);
    }

    public override void OnRemove()
    {
        dispatcher.RemoveListener(CommandEvents.START_SPAWN_UNIT, OnStartSpawnUnit);
        dispatcher.RemoveListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_DOWN, OnBuildTurretMouseDown);
        dispatcher.RemoveListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_UP, OnBuildTurretMouseUp);
        dispatcher.RemoveListener(MediatorEvents.UNIT_DESTROY_ANIMATION_FINISHED, OnUnitDestroyAnimationFinished);
        unitsModel.UnitRemoved -= OnUnitRemoved;
        levelModel.Teleporting -= OnTeleporting;
        levelModel.GoalCountUpdated -= OnGoalCountUpdated;
    }

    private void OnTeleporting(Vector2Int from, Vector2Int to)
    {
        //AudioManager.Instance.Play(SoundId.PortalEnter);
        AudioManager.Instance.Play(SoundId.PortalExit);
        gridView.GetTeleportCellView(from).PlayTeleportOutAnimation();
        gridView.GetTeleportCellView(to).PlayTeleportInAnimation();
    }

    private void UpdateGoalCount()
    {
        foreach (var goalCell in levelModel.GoalCells)
        {
            var goalBaseView = gridView.GetGoalBaseCellView(goalCell.CellPosition);
            goalBaseView.SetGoalNum(levelModel.GoalCount);

            var accuracyRate = levelModel.GetAccuracyRate();
            if (accuracyRate <= 1)
            {
                goalBaseView.SetColorMode(GoalBaseColor.CriticalDamage);
            }
            else if (accuracyRate <= 2)
            {
                goalBaseView.SetColorMode(GoalBaseColor.LiteDamage);
            }
            else
            {
                goalBaseView.SetColorMode(GoalBaseColor.Default);
            }
        }
    }

    private void OnGoalCountUpdated()
    {
        UpdateGoalCount();
    }

    private void OnUnitRemoved(UnitModel unitModel)
    {
        modelByView.Remove(unitModel);
    }

    private void OnUnitDestroyAnimationFinished(IEvent payload)
    {
        var unitView = (UnitView)payload.data;
        Destroy(unitView.gameObject);
    }

    private void OnBuildTurretMouseDown(IEvent payload)
    {
        if (_flyingTurret != null)
        {
            Destroy(_flyingTurret);
        }
        _flyingTurretType = (TurretType)payload.data;
        var turretConfig = turretsConfigProvider.GetConfig(_flyingTurretType, 0);
        FlyingTurretConfigProvider.SetConfig(turretConfig);
        _flyingTurret = Instantiate(turretConfig.BuildModePrefab, rootTransformProvider.transform);
        var pos = _flyingTurret.transform.position;
        pos.y = -5;
        _flyingTurret.transform.position = pos;
    }

    private void OnBuildTurretMouseUp(IEvent payload)
    {
        if (_flyingTurret != null)
        {
            Destroy(_flyingTurret);
            _flyingTurret = null;

            var isBuildCanceled = (bool)payload.data;
            if (!isBuildCanceled)
            {
                var offset = new Vector3(0, 0, cellSizeProvider.CellSize.y);
                dispatcher.Dispatch(MediatorEvents.REQUEST_BUILD_TURRET,
                    new MediatorEventsParams.RequestBuildParams(_flyingTurretType, gridView.WorldToCell(worldMousePositionProvider.Position + offset)));
            } else
            {
                AudioManager.Instance.Play(SoundId.BuildTurretCanceled);
            }
        }
    }

    private async void OnStartSpawnUnit(IEvent payload)
    {
        var unitModel = (UnitModel)payload.data;
        var baseCellView = gridView.GetSpawnCellView(unitModel.CurrentCellPosition);
        GameObject unitGo = null;
        void PLatformBottomPointReached()
        {
            unitGo = Instantiate(unitModel.Prefab, baseCellView.SpawnPoint);
            var lookVector = gridView.CellVec2ToWorld(unitModel.NextCellPosition) - gridView.CellVec2ToWorld(unitModel.CurrentCellPosition);
            unitGo.transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
            var unitView = unitGo.GetComponent<UnitView>();
            modelByView.Add(unitView, unitModel);
        }

        baseCellView.PLatformBottomPointReached += PLatformBottomPointReached;
        await baseCellView.PlaySpawnAnimationAsync();
        baseCellView.PLatformBottomPointReached -= PLatformBottomPointReached;

        if (unitGo != null)
        {
            unitGo.transform.SetParent(gridView.transform, true);

            dispatcher.Dispatch(MediatorEvents.UNIT_SPAWNED, unitModel);
        }
        else
        {
            Debug.Log("Spawn finished with null unit");
        }
    }

    private void Start()
    {
        DrawGrid();

        UpdateGoalCount();

        dispatcher.Dispatch(MediatorEvents.DRAW_GRID_COMPLETE);
    }

    private void DrawGrid()
    {
        if (levelModel.IsTransposed)
        {
            gridView.SetTransposed(levelModel.IsTransposed);
        }

        foreach (var cellDataMin in levelModel.Cells)
        {
            var config = cellConfigProvider.GetConfig(cellDataMin.CellConfigMin.CellType, cellDataMin.CellConfigMin.CellSubType);
            var cellType = config.CellConfigMin.CellType;
            gridView.DrawCell(cellDataMin.CellPosition, config.Prefab,
                CellInfoHelper.IsRotatableCell(cellType, config.CellConfigMin.CellSubType),
                cellType == CellType.Ground || cellType == CellType.Wall);
        }

        foreach (var cellDataMin in levelModel.Modifiers)
        {
            var cellType = cellDataMin.CellConfigMin.CellType;
            var cellSubType = cellDataMin.CellConfigMin.CellSubType;
            var config = cellConfigProvider.GetConfig(cellType, cellSubType);
            var canBeRotated = CellInfoHelper.IsRotatableCell(cellType, cellSubType);
            gridView.DrawModifier(cellDataMin.CellPosition, config.Prefab, canBeRotated);
        }

        gridView.BakeStatic();
    }
}
