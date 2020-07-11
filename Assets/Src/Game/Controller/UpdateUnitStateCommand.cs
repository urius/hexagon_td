public class UpdateUnitStateCommand : ParamCommand<UnitModel>
{
    [Inject] public LevelModel LevelModel { get; set; }

    public override void Execute(UnitModel unit)
    {
        // закончили двигаться от точки возрождения
        if (unit.CurrentStateName == UnitStateName.Moving && (unit.PreviousStateName == UnitStateName.Spawning || LevelModel.IsTeleport(unit.PreviousCellPosition)))
        {
            LevelModel.FreeCell(unit.PreviousCellPosition);
        }

        if (unit.IsOnLastCell)
        {
            unit.SetState(new DestroingState(unit.CurrentCellPosition));
            LevelModel.RemoveUnit(unit);
        }
        else if (LevelModel.IsCellFree(unit.NextCellPosition))
        {
            var fromPosition = unit.CurrentCellPosition;

            unit.IncrementCellIndex();

            var newState = new MovingState(unit.CurrentCellPosition, fromPosition);
            if (unit.CurrentStateName != UnitStateName.Spawning && !LevelModel.IsTeleport(fromPosition))
            {
                LevelModel.FreeCell(fromPosition);
            }
            LevelModel.OwnCellByUnit(unit);

            unit.SetState(newState);
        }
        else
        {
            LevelModel.OwnCellByUnit(unit);
            unit.SetState(new WaitingState(unit.CurrentCellPosition, unit.NextCellPosition));
        }
    }
}
