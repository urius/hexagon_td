public class RequestBuildTurretCommand : ParamCommand<MediatorEventsParams.RequestBuildParams>
{
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public TurretConfigProvider TurretConfigProvider { get; set; }

    public RequestBuildTurretCommand()
    {
    }

    public override void Execute(MediatorEventsParams.RequestBuildParams data)
    {
        if (LevelModel.IsReadyToBuild(data.GridPosition))
        {
            var turretConfig = TurretConfigProvider.GetConfig(data.TurretType, 0);
            var turretModel = new TurretModel(turretConfig, data.GridPosition);
            LevelModel.LevelTurretsModel.AddTurret(turretModel);

            switch (turretModel.TurretType)
            {
                case TurretType.Gun:
                    injectionBinder.GetInstance<GunTurretMediator>()
                        .Initialize(turretModel);
                    break;
                case TurretType.Laser:
                    injectionBinder.GetInstance<LaserTurretMediator>()
                        .Initialize(turretModel);
                    break;
            }
        }
    }
}