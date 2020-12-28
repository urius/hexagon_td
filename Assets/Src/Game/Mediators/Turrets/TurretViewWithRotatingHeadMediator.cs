using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TurretViewWithRotatingHeadMediator : TurretMediatorBase
{
    [Inject] public IViewManager ViewManager { get; set; }
    [Inject] public IRootTransformProvider RootTransformProvider { get; set; }
    [Inject] public IUnitViewsProvider UnitViewsProvider { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }

    protected TurretViewWithRotatingHead TurretView;
    protected UnitView TargetView;
    protected bool TargetIsLocked;

    private int _unitIndex;
    private IList<UnitView> _unitViews;
    private int _currentFirePointIndex = 0;
    private Transform _targetTransform;
    private float _rotationSpeedDegrees;

    protected TurretConfig TurretConfig => TurretModel.TurretConfig;
    protected bool IsTargetInAttackZone => TargetView != null ?
        Vector3.SqrMagnitude(TargetView.transform.position - SelfPosition) <= AttackRadiusSqr : false;
    protected bool IsLookOnTarget
    {
        get
        {
            var targetRotation = GetTargetRotation(_targetTransform);
            return Quaternion.Angle(TurretView.HeadRotation, targetRotation) < _rotationSpeedDegrees;
        }
    }

    protected override void Activate()
    {
        _unitViews = UnitViewsProvider.UnitViews;

        base.Activate();

        _rotationSpeedDegrees = TurretConfig.RotationSpeedDegrees;

        TurretModel.NewTargetSet += OnNewTargetSet;
    }

    protected override void Deactivate()
    {
        base.Deactivate();

        TurretModel.NewTargetSet -= OnNewTargetSet;
    }

    protected override void OnUpdate()
    {
        if (TargetView == null)
        {
            ProcessCheckNextUnitInAttackZone();
        }
        else
        {

            if (!TargetIsLocked && IsLookOnTarget)
            {
                TargetIsLocked = true;
                dispatcher.Dispatch(MediatorEvents.TURRET_TARGET_LOCKED, TurretModel);
            }

            if (!IsTargetInAttackZone)
            {
                var param = new MediatorEventsParams.TurretUnitLeaveAttackZoneParams(TurretModel, TurretModel.TargetUnit);
                dispatcher.Dispatch(MediatorEvents.TURRET_UNIT_LEAVE_ATTACK_ZONE, param);
            } else
            {
                ProcessHeadRotation();
            }
        }
    }

    protected Transform GetFirePoint()
    {
        if (_currentFirePointIndex >= TurretView.FirePoints.Length)
        {
            _currentFirePointIndex = 0;
        }
        return TurretView.FirePoints[_currentFirePointIndex++].transform;
    }

    override protected void OnTurretUpgraded()
    {
        var turretRotationBuf = TurretView.HeadRotation;
        TargetIsLocked = false;

        base.OnTurretUpgraded();

        TurretView.HeadRotation = turretRotationBuf;
    }

    protected override GameObject CreateView(TurretModel turretModel)
    {
        var turretPrefab = TurretsConfigProvider.GetConfig(turretModel.TurretType, turretModel.TurretConfig.TurretLevelIndex).Prefab;
        var turretViewGo = GameObject.Instantiate(turretPrefab, RootTransformProvider.transform);
        turretViewGo.transform.position = CellPositionConverter.CellVec2ToWorld(turretModel.Position);
        TurretView = turretViewGo.GetComponent<TurretViewWithRotatingHead>();
        _targetTransform = GetTransformForTargeting(TargetView);
        return turretViewGo;
    }

    protected virtual Transform GetTransformForTargeting([CanBeNull] UnitView unitView)
    {
        return unitView?.transform;
    }

    protected virtual void ShowSparks(Vector3 point, Vector3 direction)
    {
        ViewManager.Instantiate(TurretConfig.BulletSparksPrefab, point, Quaternion.LookRotation(direction));
    }

    private void ProcessHeadRotation()
    {
        if (_targetTransform != null)
        {
            var targetRotation = GetTargetRotation(_targetTransform);
            TurretView.HeadRotation = Quaternion.RotateTowards(TurretView.HeadRotation, targetRotation, _rotationSpeedDegrees);
        }
    }

    private Quaternion GetTargetRotation(Transform targetTransform)
    {
        var lookVector = targetTransform.position - TurretView.HeadPosition;
        lookVector.y = 0;
        return Quaternion.LookRotation(lookVector, Vector3.up);
    }

    private void OnNewTargetSet()
    {
        if (TurretModel.TargetUnit != null)
        {
            TargetView = UnitViewsProvider.GetViewByModel(TurretModel.TargetUnit);
            _targetTransform = GetTransformForTargeting(TargetView);
        }
        else
        {
            TargetView = null;
            _targetTransform = null;
        }

        TargetIsLocked = false;
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
