using System.Collections.Generic;
using UnityEngine;

public class TurretViewWithRotationgHeadMediator : TurretMediatorBase
{
    protected TurretViewWithRotatingHead TurretView;
    protected UnitView TargetView;

    private int _unitIndex;
    private bool _targetIsLocked;
    private IList<UnitView> _unitViews;

    protected TurretConfig TurretConfig => TurretModel.TurretConfig;

    protected override void Activate()
    {
        _unitViews = UnitViewsProvider.UnitViews;

        base.Activate();

        TurretModel.NewTargetSet += OnNewTargetSet;
        UpdateProvider.UpdateAction += OnUpdate;
    }

    protected override void Deactivate()
    {
        base.Deactivate();

        TurretModel.NewTargetSet -= OnNewTargetSet;
        UpdateProvider.UpdateAction -= OnUpdate;
    }

    protected virtual void OnUpdate()
    {
        if (TargetView == null)
        {
            ProcessCheckNextUnitInAttackZone();
        }
        else
        {
            if (!_targetIsLocked && TurretView.IsLookOnTarget)
            {
                _targetIsLocked = true;
                dispatcher.Dispatch(MediatorEvents.TURRET_TARGET_LOCKED, TurretModel);
            }

            if (Vector3.SqrMagnitude(TargetView.transform.position - SelfPosition) > AttackRadiusSqr)
            {
                dispatcher.Dispatch(MediatorEvents.TURRET_TARGET_LEAVE_ATTACK_ZONE, TurretModel);
            }
        }
    }

    override protected void OnTurretUpgraded()
    {
        var turretRotationBuf = TurretView.TurretRotation;
        _targetIsLocked = false;

        base.OnTurretUpgraded();

        TurretView.TurretRotation = turretRotationBuf;
    }

    protected override GameObject CreateView(TurretModel turretModel)
    {
        var turretPrefab = TurretsConfigProvider.GetConfig(turretModel.TurretType, turretModel.TurretConfig.TurretLevelIndex).Prefab;
        var turretViewGo = GameObject.Instantiate(turretPrefab, CellPositionConverter.CellVec2ToWorld(turretModel.Position), Quaternion.identity);
        TurretView = turretViewGo.GetComponent<TurretViewWithRotatingHead>();
        TurretView.SetTargetTransform(TargetView?.transform);
        return turretViewGo;
    }

    private void OnNewTargetSet()
    {
        if (TurretModel.TargetUnit != null)
        {
            TargetView = UnitViewsProvider.GetViewByModel(TurretModel.TargetUnit);
            TurretView.SetTargetTransform(TargetView.transform);
        }
        else
        {
            TargetView = null;
            TurretView.SetTargetTransform(null);
        }

        _targetIsLocked = false;
    }

    private void ProcessCheckNextUnitInAttackZone()
    {
        if (_unitViews.Count > 0)
        {
            if (_unitIndex >= _unitViews.Count)
            {
                _unitIndex = 0;
            }

            var distanceToUnitSqr = Vector3.SqrMagnitude(_unitViews[_unitIndex].transform.position - SelfPosition);
            if (distanceToUnitSqr < AttackRadiusSqr)
            {
                var param = new MediatorEventsParams.TurretUnitInAttackZoneParams(TurretModel, UnitModelByViews.GetModel(_unitViews[_unitIndex]));
                dispatcher.Dispatch(MediatorEvents.TURRET_DETECTED_UNIT_IN_ATTACK_ZONE, param);

                _unitIndex = 0;
                return;
            }
            _unitIndex++;
        }
    }
}
