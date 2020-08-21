public class TurretUpgradeClickedCommand : ParamCommand<TurretModel>
{
    [Inject] public LevelTurretsModel LevelTurretsModel { get; set; }
    [Inject] public TurretConfigProvider TurretConfigProvider { get; set; }

    public TurretUpgradeClickedCommand()
    {
    }

    public override void Execute(TurretModel turret)
    {
        var upgradedTurretConfig = TurretConfigProvider.GetConfig(turret.TurretType, turret.TurretConfig.TurretLevelIndex + 1);
        if (upgradedTurretConfig != null)
        {
            turret.Upgrade(upgradedTurretConfig);
        }
    }
}
