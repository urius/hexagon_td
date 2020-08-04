using System;
public class ChooseTurretTargetCommand : ParamCommand<MediatorEventsParams.TurretUnitInAttackZoneParams>
{

    public override void Execute(MediatorEventsParams.TurretUnitInAttackZoneParams data)
    {
        data.TurretModel.SetTarget(data.UnitModel);
    }
}
