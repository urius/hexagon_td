public class UpdateUnitStateCommand : ParamCommand<UnitModel>
{
    [Inject] public LevelModel LevelModel { get; set; }

    public override void Execute(UnitModel unit)
    {
        if (unit.IsOnLastCell)
        {
            unit.SetState(new DestroingState(unit.CurrentCellPosition));
            LevelModel.RemoveUnit(unit);
        }
        else if (LevelModel.IsCellFree(unit.NextCellPosition))
        {
            unit.IncrementCellIndex();
            LevelModel.OwnCellByUnit(unit);
            unit.SetState(new MovingState(unit.CurrentCellPosition, unit.PreviousCellPosition));
        }
        else
        {
            LevelModel.OwnCellByUnit(unit);
            unit.SetState(new WaitingState(unit.CurrentCellPosition, unit.NextCellPosition));
        }
    }
}
