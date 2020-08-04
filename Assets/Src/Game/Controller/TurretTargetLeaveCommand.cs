using System;
public class TurretTargetLeaveCommand : ParamCommand<TurretModel>
{
    public override void Execute(TurretModel turret)
    {
        turret.SetTarget(null);
    }
}
