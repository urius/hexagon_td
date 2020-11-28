using System;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class UnitsControlSystem : EventSystemBase
{
    [Inject] public IUpdateProvider UpdateProvider { get; set; }
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public UnitConfigsProvider UnitConfigsProvider { get; set; }
    [Inject] public WaveModel WaveModel { get; set; }
    [Inject] public LevelTurretsModel LevelTurretsModel { get; set; }

    private int _framesCount = 0;

    public UnitsControlSystem()
    {
    }

    public override void Start()
    {
        UpdateProvider.UpdateAction += OnUpdate;
        LevelTurretsModel.TurretAdded += OnTurretAdded;
        LevelTurretsModel.TurretRemoved += OnTurretRemoved;

        dispatcher.AddListener(MediatorEvents.UNIT_SPAWNED, OnUnitSpawnAnimationEnded);
        dispatcher.AddListener(MediatorEvents.UNIT_MOVE_TO_NEXT_CELL_FINISHED, OnUnitMoveToCellFinished);
        dispatcher.AddListener(MediatorEvents.UNIT_HALF_STATE_PASSED, OnUnitRequestFreeCell);
    }

    private void OnTurretRemoved(TurretModel turret)
    {
        foreach (var unit in LevelUnitsModel.Units)
        {
            if (!unit.IsOnLastCell && !unit.IsOnPreLastCell)
            {
                var (currentCell, finishCell) = unit.GetRestPathEdgePoints();
                var path = LevelModel.PathsManager.GetPath(currentCell, finishCell);
                unit.SubstitutePath(path);
            }
        }
    }

    private void OnTurretAdded(TurretModel turret)
    {
        //Update pathes for units affected by new turret
        foreach (var unit in LevelUnitsModel.Units)
        {
            if (unit.IsRestPathContainsCell(turret.Position))
            {
                var (currentCell, finishCell) = unit.GetRestPathEdgePoints();
                var path = LevelModel.PathsManager.GetPath(currentCell, finishCell);
                if (path.Count <= 0)
                {
                    unit.SetDestroingState();
                }
                else
                {
                    unit.SubstitutePath(path);
                }
            }
        }
    }

    private void OnUpdate()
    {
        _framesCount++;
        if (_framesCount > 20)
        {
            _framesCount = 0;

            foreach (var waitingUnit in LevelUnitsModel.WaitingUnits)
            {
                if (LevelUnitsModel.IsCellWithoutUnit(waitingUnit.NextCellPosition))
                {
                    UpdateUnitState(waitingUnit);
                }
            }

            if (WaveModel.WaveState == WaveState.InWave)
            {
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        var haveActiveCells = false;

        for (var i = 0; i < LevelModel.SpawnCells.Count; i++)
        {
            var spawnCell = LevelModel.SpawnCells[i];
            if (WaveModel.IsBaseAllowedToSpawn(i))
            {
                haveActiveCells = true;
                if (!WaveModel.IsCurrentWaveEmpty && LevelUnitsModel.IsCellWithoutUnit(spawnCell.CellPosition))
                {
                    var unitData = WaveModel.GetUnitAndIncrement();
                    var unitType = unitData.Item1;
                    var unitConfig = UnitConfigsProvider.GetConfigByType(unitType);

                    var unitSkinType = unitData.Item2 == UnitSkin.None ? unitConfig.Skin : unitData.Item2;
                    var unitSkinConfig = UnitConfigsProvider.GetSkinConfigBySkin(unitSkinType);

                    //injectionBinder.Unbind<UnitModel>();
                    //injectionBinder.Bind<UnitModel>().ToValue(unitModel);
                    //  OnRegister happens on next frame, so this workaround is not working

                    var path = LevelModel.GetPath(spawnCell.CellPosition);
                    var unitModel = new UnitModel(path, unitConfig, unitSkinConfig);
                    LevelUnitsModel.AddUnit(unitModel);
                    LevelUnitsModel.OwnCellByUnit(unitModel);

                    dispatcher.Dispatch(CommandEvents.START_SPAWN_UNIT, unitModel);
                }
            }
        }

        if (!haveActiveCells)
        {
            Debug.LogError($"Wave {WaveModel.WaveIndex + 1} have no bases allowed to spawn");
        }
    }

    public void UpdateUnitState(UnitModel unit)
    {
        if (unit.IsOnLastCell)
        {
            LevelModel.SubstractGoalCapacity();
            LevelUnitsModel.DestroyUnit(unit);
            dispatcher.Dispatch(CommandEvents.UI_REQUEST_SHAKE_CAMERA);
        }
        else if (LevelUnitsModel.IsCellWithoutUnit(unit.NextCellPosition))
        {
            var newState = new MovingState(unit.NextCellPosition, unit.CurrentCellPosition, LevelModel.GetSpeedMultiplier(unit.NextCellPosition));
            unit.SetState(newState);
            LevelUnitsModel.OwnCellByUnit(unit);

            if (newState.IsTeleporting)
            {
                LevelModel.DispatchTeleporting(unit.PreviousCellPosition, unit.CurrentCellPosition);
            }
        }
        else if (unit.CurrentStateName != UnitStateName.WaitingForCell && !unit.IsDestroying)
        {
            LevelUnitsModel.OwnCellByUnit(unit);
            unit.SetState(new WaitingState(unit.CurrentCellPosition, unit.NextCellPosition));
        }
    }

    private void OnUnitSpawnAnimationEnded(IEvent payload)
    {
        UpdateUnitState(payload.data as UnitModel);
    }

    private void OnUnitMoveToCellFinished(IEvent payload)
    {
        var unitModel = payload.data as UnitModel;
        AffectCellModifier(unitModel);
        if (!unitModel.IsDestroying)
        {
            UpdateUnitState(unitModel);
        }
    }

    private void AffectCellModifier(UnitModel unit)
    {
        if (LevelModel.TryGetModifier(unit.CurrentCellPosition, out var modifier))
        {
            switch (modifier)
            {
                case ModifierType.Repair:
                    unit.Repair(LevelModel.ModifierRepairValue);
                    break;
                case ModifierType.Mine:
                    LevelUnitsModel.ApplyDamageToUnit(unit, LevelModel.ModifierMineDamage);
                    break;
                case ModifierType.ExtraMoney:
                    ProcessExtraMoneyModifier(unit, LevelModel.ModifierMoneyAmount);
                    break;
                case ModifierType.ExtraBigMoney:
                    ProcessExtraMoneyModifier(unit, LevelModel.ModifierBigMoneyAmount);
                    break;
            }
        }
    }

    private void ProcessExtraMoneyModifier(UnitModel unit, int moneyAmount)
    {
        unit.ShowEarnedMoney(moneyAmount);
        LevelModel.TryAddMoney(moneyAmount);
    }

    private void OnUnitRequestFreeCell(IEvent payload)
    {
        LevelUnitsModel.FreeCell((payload.data as UnitModel).PreviousCellPosition);
    }
}
