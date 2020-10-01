using System;
using strange.extensions.dispatcher.eventdispatcher.api;

public class UnitsControlSystem : EventSystemBase
{
    [Inject] public IUpdateProvider UpdateProvider { get; set; }
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public UnitConfigsProvider UnitConfigsProvider { get; set; }
    [Inject] public WaveModel WaveModel { get; set; }

    private int _framesCount = 0;

    public UnitsControlSystem()
    {
    }

    public override void Start()
    {
        UpdateProvider.UpdateAction += OnUpdate;
        dispatcher.AddListener(MediatorEvents.UNIT_SPAWNED, OnUnitSpawnAnimationEnded);
        dispatcher.AddListener(MediatorEvents.UNIT_MOVE_TO_NEXT_CELL_FINISHED, OnUnitMoveToCellFinished);
        dispatcher.AddListener(MediatorEvents.UNIT_HALF_STATE_PASSED, OnUnitRequestFreeCell);
        dispatcher.AddListener(MediatorEvents.UNIT_BEFORE_ROTATION, OnUnitBeforeRotation);
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
        foreach (var spawnCell in LevelModel.SpawnCells)
        {
            if (!WaveModel.IsCurrentWaveEmpty)
            {
                if (LevelUnitsModel.IsCellWithoutUnit(spawnCell.CellPosition))
                {
                    var unitType = WaveModel.GetUnitAndIncrement();
                    var unitConfig = UnitConfigsProvider.GetConfigByType(unitType);

                    //injectionBinder.Unbind<UnitModel>();
                    //injectionBinder.Bind<UnitModel>().ToValue(unitModel);
                    //  OnRegister happens on next frame, so this workaround is not working

                    var path = LevelModel.GetPath(spawnCell.CellPosition);
                    var unitModel = new UnitModel(path, unitConfig);
                    LevelUnitsModel.AddUnit(unitModel);
                    LevelUnitsModel.OwnCellByUnit(unitModel);

                    dispatcher.Dispatch(CommandEvents.START_SPAWN_UNIT, unitModel);
                }
            }
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
        else if (unit.CurrentStateName != UnitStateName.WaitingForCell)
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
        UpdateUnitState(payload.data as UnitModel);
    }

    private void OnUnitBeforeRotation(IEvent payload)
    {
        AffectCellModifier(payload.data as UnitModel);
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
