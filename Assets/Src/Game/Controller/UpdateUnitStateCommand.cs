using UnityEngine;

public class UpdateUnitStateCommand : ParamCommand<UnitModel>
{
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }

    public override void Execute(UnitModel unit)
    {
        if (unit.IsOnLastCell)
        {
            unit.SetState(new DestroingState(unit.CurrentCellPosition));
            LevelUnitsModel.RemoveUnit(unit);
            dispatcher.Dispatch(CommandEvents.UNIT_DESTROYING, unit);
        }
        else if (LevelUnitsModel.IsCellWithoutUnit(unit.NextCellPosition))
        {
            unit.IncrementCellIndex();
            LevelUnitsModel.OwnCellByUnit(unit);
            var newState = new MovingState(unit.CurrentCellPosition, unit.PreviousCellPosition);
            unit.SetState(newState);

            if (newState.IsTeleporting)
            {
                LevelModel.DispatchTeleporting(unit.PreviousCellPosition, unit.CurrentCellPosition);
            }
        }
        else
        {
            LevelUnitsModel.OwnCellByUnit(unit);
            unit.SetState(new WaitingState(unit.CurrentCellPosition, unit.NextCellPosition));
        }
    }
}
