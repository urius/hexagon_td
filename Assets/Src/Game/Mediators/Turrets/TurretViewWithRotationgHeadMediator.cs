using System;
using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class TurretViewWithRotationgHeadMediator
{
    [Inject(ContextKeys.CONTEXT_DISPATCHER)]
    public IEventDispatcher dispatcher { get; set; }

    [Inject] public IUnitViewsProvider UnitViewsProvider { get; set; }
    [Inject] public IUnitModelByViewProvider UnitModelByViews { get; set; }
    [Inject] public ICellSizeProvider CellSizeProvider { get; set; }
    [Inject] public IUpdateProvider UpdateProvider { get; set; }
    [Inject] public IViewManager ViewManager { get; set; }
    [Inject] public TurretConfigProvider TurretsConfigProvider { get; set; }
    [Inject] public ICellPositionConverter CellPositionConverter { get; set; }
    [Inject] public UIPrefabsConfig UIPrefabsConfig { get; set; }

    protected TurretModel TurretModel;
    protected TurretViewWithRotatingHead TurretView;
    protected UnitView TargetView;

    private double _attackRadiusSqr;
    private int _unitIndex;
    private IList<UnitView> _unitViews;
    private bool _targetIsLocked;
    private Vector3 _selfPosition;
    private TurretRadiusView _turretRadius;
    private GameObject _turretSelection;

    protected TurretConfig TurretConfig => TurretModel.TurretConfig;

    public void Initialize(TurretModel turretModel)
    {
        TurretModel = turretModel;
        TurretView = CreateView(TurretModel);
        OnRegister();
    }

    protected virtual void OnRegister()
    {
        _unitViews = UnitViewsProvider.UnitViews;

        RefreshAttackRadius();

        TurretModel.NewTargetSet += OnNewTargetSet;
        TurretModel.Upgraded += OnTurretUpgraded;
        UpdateProvider.UpdateAction += OnUpdate;
        dispatcher.AddListener(CommandEvents.TURRET_SELECTED, OnTurretSelected);
        dispatcher.AddListener(MediatorEvents.TURRET_DESELECTED, OnTurretDeselected);

        _selfPosition = TurretView.transform.position;
    }

    protected virtual void OnTurretUpgraded()
    {
        TargetView = null;
        _targetIsLocked = false;
        RefreshAttackRadius();
        var turretRotationBuf = TurretView.TurretRotation;

        GameObject.DestroyImmediate(TurretView.gameObject);
        TurretView = CreateView(TurretModel);
        TurretView.TurretRotation = turretRotationBuf;

        GameObject.Instantiate(UIPrefabsConfig.UpgradePSPrefab, TurretView.transform.position, Quaternion.identity);
    }

    private void RefreshAttackRadius()
    {
        _attackRadiusSqr = Math.Pow(TurretModel.AttackRadiusCells * CellSizeProvider.CellSize.x, 2);
    }

    private TurretViewWithRotatingHead CreateView(TurretModel turretModel)
    {
        var turretPrefab = TurretsConfigProvider.GetConfig(turretModel.TurretType, turretModel.TurretConfig.TurretLevelIndex).Prefab;
        var turretViewGo = GameObject.Instantiate(turretPrefab, CellPositionConverter.CellVec2ToWorld(turretModel.Position), Quaternion.identity);
        return turretViewGo.GetComponent<TurretViewWithRotatingHead>();
    }

    /*public void OnRemove()
    {
        _turretModel.NewTargetSet -= OnNewTargetSet;
        _turretModel.Fired -= OnFired;
        UpdateProvider.UpdateAction -= OnUpdate;    
        TurretModel.Upgraded -= OnTurretUpgraded;
        dispatcher.RemoveListener(CommandEvents.TURRET_SELECTED, OnTurretSelected);
        dispatcher.RemoveListener(MediatorEvents.TURRET_DESELECTED, OnTurretDeselected);
    }
    */

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

    private void OnTurretSelected(IEvent payload)
    {
        var turetModel = (TurretModel)payload.data;

        DestroyTurretRadiusView();
        DestroyTurretSelectionView();
        if (TurretModel == turetModel)
        {
            var turretRadiusGo = GameObject.Instantiate(UIPrefabsConfig.TurretRadiusPrefab);
            _turretRadius = turretRadiusGo.GetComponent<TurretRadiusView>();
            _turretRadius.transform.position = _selfPosition;
            _turretRadius.SetSize(2 * (float)Math.Sqrt(_attackRadiusSqr));

            _turretSelection = GameObject.Instantiate(UIPrefabsConfig.TurretSelectionPrefab);
            _turretSelection.transform.position = _selfPosition;
        }
    }

    private void DestroyTurretRadiusView()
    {
        if (_turretRadius != null)
        {
            GameObject.Destroy(_turretRadius.gameObject);
            _turretRadius = null;
        }
    }

    private void DestroyTurretSelectionView()
    {
        if (_turretSelection != null)
        {
            GameObject.Destroy(_turretSelection);
            _turretSelection = null;
        }
    }


    private void OnTurretDeselected(IEvent payload)
    {
        DestroyTurretRadiusView();
        DestroyTurretSelectionView();
    }

    private void OnUpdate()
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

            if (Vector3.SqrMagnitude(TargetView.transform.position - _selfPosition) > _attackRadiusSqr)
            {
                dispatcher.Dispatch(MediatorEvents.TURRET_TARGET_LEAVE_ATTACK_ZONE, TurretModel);
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

            var distanceToUnitSqr = Vector3.SqrMagnitude(_unitViews[_unitIndex].transform.position - _selfPosition);
            if (distanceToUnitSqr < _attackRadiusSqr)
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
