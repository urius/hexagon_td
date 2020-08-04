using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class TurretViewWithRotationgHeadMediator : EventMediator
{
    [Inject] public TurretViewWithRotatingHead TurretView { get; set; }
    [Inject] public IUnitViewsProvider UnitViewsProvider { get; set; }
    [Inject] public IUnitModelByViewProvider UnitModelByViews { get; set; }
    [Inject] public ITurretModelByViewProvider TurretModelByView { get; set; }
    [Inject] public GridViewProvider GridViewProvider { get; set; }
    [Inject] public IUpdateProvider UpdateProvider { get; set; }
    [Inject] public IViewManager ViewManager { get; set; }

    private float _attackRadius;
    private TurretModel _turretModel;
    private UnitView _targetView;
    private int _unitIndex;
    private IList<UnitView> _unitViews;
    private bool _targetIsLocked;
    private Vector3 _selfPosition;

    public override void OnRegister()
    {
        base.OnRegister();

        _turretModel = TurretModelByView.GetModel(TurretView);
        _unitViews = UnitViewsProvider.UnitViews;

        var cellWidth = GridViewProvider.GridView.CellSize.x;
        _attackRadius = _turretModel.AttackRadiusCells * cellWidth;

        _turretModel.NewTargetSet += OnNewTargetSet;
        _turretModel.Fired += OnFired;
        UpdateProvider.UpdateAction += OnUpdate;

        _selfPosition = TurretView.transform.position;
    }

    public override void OnRemove()
    {
        base.OnRemove();

        _turretModel.NewTargetSet -= OnNewTargetSet;
        _turretModel.Fired -= OnFired;
        UpdateProvider.UpdateAction -= OnUpdate;
    }

    private void OnNewTargetSet()
    {
        if (_turretModel.TargetUnit != null)
        {
            _targetView = UnitViewsProvider.GetViewByModel(_turretModel.TargetUnit);
            TurretView.SetTargetTransform(_targetView.transform);
        }
        else
        {
            _targetView = null;
            TurretView.SetTargetTransform(null);
        }

        _targetIsLocked = false;
    }

    private void OnUpdate()
    {
        if (_targetView == null)
        {
            ProcessCheckNextUnitInAttackZone();
        }
        else
        {
            if (!_targetIsLocked && TurretView.IsLookOnTarget)
            {
                _targetIsLocked = true;
                dispatcher.Dispatch(MediatorEvents.TURRET_TARGET_LOCKED, _turretModel);
            }

            if (Vector3.Distance(_targetView.transform.position, _selfPosition) > _attackRadius)
            {
                dispatcher.Dispatch(MediatorEvents.TURRET_TARGET_LEAVE_ATTACK_ZONE, _turretModel);
            }
        }
    }

    private void ProcessCheckNextUnitInAttackZone()
    {
        if (_unitViews.Count > 0)
        {
            if (_unitIndex >= _unitViews.Count)
            {
                _unitIndex = 0;
            }

            var distanceToUnit = Vector3.Distance(_unitViews[_unitIndex].transform.position, _selfPosition);
            if (distanceToUnit < _attackRadius)
            {
                var param = new MediatorEventsParams.TurretUnitInAttackZoneParams(_turretModel, UnitModelByViews.GetModel(_unitViews[_unitIndex]));
                dispatcher.Dispatch(MediatorEvents.TURRET_DETECTED_UNIT_IN_ATTACK_ZONE, param);

                _unitIndex = 0;
                return;
            }
            _unitIndex++;
        }
    }

    private void OnFired()
    {
        Debug.Log("OnFired");

        var firePoint = TurretView.FirePoints[0].transform;
        var bulletGo = ViewManager.Instantiate(_turretModel.TurretConfig.BulletPrefab, firePoint.position, firePoint.rotation);
        var bullet = bulletGo.GetComponent<BulletBase>();
        bullet.Setup(_targetView, _turretModel.TargetUnit, dispatcher, ViewManager);
    }
}
