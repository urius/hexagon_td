public class UpdateUnitStateCommand : ParamCommand<UnitModel>
{
    public override void Execute(UnitModel unit)
    {
        unit.AdvanceState();
    }
}
