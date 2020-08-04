using System;
public class TurretLockTargetCommand : ParamCommand<TurretModel>
{
    public override void Execute(TurretModel turret)
    {
        turret.LockTarget();
    }
}
