public class TurretSellClickedCommand : ParamCommand<TurretModel>
{
    [Inject] public LevelTurretsModel LevelTurretsModel { get; set; }

    public TurretSellClickedCommand()
    {
    }

    public override void Execute(TurretModel turret)
    {
        turret.Destroy();
        LevelTurretsModel.RemoveTurret(turret);
        //TODO: add money
    }
}
