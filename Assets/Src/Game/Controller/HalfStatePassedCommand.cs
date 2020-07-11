public class HalfStatePassedCommand : ParamCommand<UnitModel>
{
    [Inject] public LevelModel LevelModel { get; set; }

    public override void Execute(UnitModel unit)
    {
        LevelModel.FreeCell(unit.PreviousCellPosition);        
    }
}
