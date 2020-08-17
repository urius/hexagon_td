public class FlyingTurretConfigProvider
{
    public TurretConfig TurretConfig { get; private set; }

    public void SetConfig(TurretConfig turretConfig)
    {
        TurretConfig = turretConfig;
    }
}
