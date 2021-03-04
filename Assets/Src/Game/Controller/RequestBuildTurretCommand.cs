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
            if (!LevelModel.TrySubstractMoney(turretConfig.Price))
            {
                LevelModel.TriggerInsufficientMoney();
                AudioManager.Instance.Play(SoundId.TurretUnableToPlace);
                return;
            }

            AudioManager.Instance.Play(SoundId.TurretPlaced);

            var boosterValues = PlayerSessionModel.Instance.SelectedLevelData.BoosterValues;
            var turretModel = new TurretModel(turretConfig, data.GridPosition, boosterValues);
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
                case TurretType.Rocket:
                    injectionBinder.GetInstance<RocketTurretMediator>()
                        .Initialize(turretModel);
                    break;
                case TurretType.SlowField:
                    injectionBinder.GetInstance<SlowFieldTurretMediator>()
                        .Initialize(turretModel);
                    break;
            }
        } else
        {
            AudioManager.Instance.Play(SoundId.TurretUnableToPlace);
        }
    }
}