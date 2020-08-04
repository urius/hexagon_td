public class HalfStatePassedCommand : ParamCommand<UnitModel>
{
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }

    public override void Execute(UnitModel unit)
    {
        LevelUnitsModel.FreeCell(unit.PreviousCellPosition);        
    }
}
