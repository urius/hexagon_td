using System;
using System.Collections.Generic;
using UnityEngine;

public class SlowFieldTurretMediator : TurretMediatorBase
{
    [Inject] public IRootTransformProvider RootTransformProvider { get; set; }
    [Inject] public IUnitViewsProvider UnitViewsProvider { get; set; }
    [Inject] public LevelUnitsModel LevelUnitsModel { get; set; }

    private SlowFieldTurretView _turretView;
    private int _unitIndex1;
    private List<UnitView> _unitViews;
    private LinkedList<UnitModel> _unitsInZone = new LinkedList<UnitModel>();

    protected override void Activate()
    {
        base.Activate();

        _unitViews = UnitViewsProvider.UnitViews;

        LevelUnitsModel.UnitRemoved += OnUnitRemoved;
        _turretView.ParticlesBurst += OnParticlesBurst;
    }

    protected override void Deactivate()
    {
        LevelUnitsModel.UnitRemoved -= OnUnitRemoved;
        _turretView.ParticlesBurst -= OnParticlesBurst;

        base.Deactivate();
    }

    protected override GameObject CreateView(TurretModel turretModel)
    {
        var turretPrefab = TurretsConfigProvider.GetConfig(turretModel.TurretType, turretModel.TurretConfig.TurretLevelIndex).Prefab;
        var turretViewGo = GameObject.Instantiate(turretPrefab, RootTransformProvider.transform);
        turretViewGo.transform.position = CellPositionConverter.CellVec2ToWorld(turretModel.Position);
        _turretView = turretViewGo.GetComponent<SlowFieldTurretView>();

        return turretViewGo;
    }

    protected override void RefreshAttackRadius()
    {
        base.RefreshAttackRadius();

        var attackRadius = (float)Math.Sqrt(AttackRadiusSqr);
        var lifetime = attackRadius / _turretView.Particles.main.startSpeed.constant;
        _turretView.SetParticlesLifetime(lifetime);
    }

    protected override void OnUpdate()
    {
        if (_unitIndex1 < _unitViews.Count)
        {
            var unitView = _unitViews[_unitIndex1];
            var unitModel = UnitModelByViews.GetModel(unitView);
            if (CheckUnitInAttackZone(unitView) && !_unitsInZone.Contains(unitModel))
            {
                _unitsInZone.AddLast(unitModel);

                var param = new MediatorEventsParams.TurretUnitInAttackZoneParams(TurretModel, unitModel);
                dispatcher.Dispatch(MediatorEvents.TURRET_DETECTED_UNIT_IN_ATTACK_ZONE, param);

                _turretView.StartFire();
            }
            _unitIndex1++;
        }
        else
        {
            _unitIndex1 = 0;
        }

        foreach (var unitModel in _unitsInZone)
        {
            if (!CheckUnitInAttackZone(UnitViewsProvider.GetViewByModel(unitModel)))
            {
                _unitsInZone.Remove(unitModel);

                var param = new MediatorEventsParams.TurretUnitLeaveAttackZoneParams(TurretModel, unitModel);
                dispatcher.Dispatch(MediatorEvents.TURRET_UNIT_LEAVE_ATTACK_ZONE, param);

                break;
            }
        }

        if (_unitsInZone.Count == 0)
        {
            _turretView.StopFire();
        }
    }

    private void OnUnitRemoved(UnitModel model)
    {
        _unitsInZone.Remove(model);
    }

    private bool CheckUnitInAttackZone(UnitView unitView)
    {
        var distanceToUnitSqr = Vector3.SqrMagnitude(unitView.transform.position - SelfPosition);

        if (distanceToUnitSqr < AttackRadiusSqr)
        {
            return true;
        }
        return false;
    }

    private void OnParticlesBurst()
    {
        AudioManager.Instance.Play(GetSoundId());
    }

    private SoundId GetSoundId()
    {
        switch (TurretModel.TurretConfig.TurretLevelIndex)
        {
            case 0:
                return SoundId.SlowField_1;
            case 1:
                return SoundId.SlowField_2;
            default:
                return SoundId.SlowField_3;
        }
    }
}
