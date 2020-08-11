using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurretMediator : TurretViewWithRotationgHeadMediator
{
    const int EndFireFrames = 10;

    private TurretConfig _turretConfig;
    private Transform _bulletTransform;
    private Transform _firePoint;
    private int EndFireFramesLeft;

    protected override void OnRegister()
    {
        base.OnRegister();

        _turretConfig = TurretModel.TurretConfig;
        _firePoint = TurretView.FirePoints[0].transform;

        TurretModel.Fired += OnFired;
    }

    private void OnFired()
    {
        if (_bulletTransform == null)
        {
            var bulletGo = ViewManager.Instantiate(TurretModel.TurretConfig.BulletPrefab, _firePoint.position, _firePoint.rotation);
            _bulletTransform = bulletGo.transform;
            var lineRenderer = bulletGo.GetComponentInChildren<LineRenderer>();
            lineRenderer.SetPosition(1, new Vector3(0, 0, _turretConfig.AttackInfluenceDistance));

            UpdateProvider.UpdateAction += OnUpdateFiring;
        }

        EndFireFramesLeft = EndFireFrames;

        CheckCollisions();
    }

    private void CheckCollisions()
    {
        var raycatHits = Physics.RaycastAll(_bulletTransform.position, _bulletTransform.forward, _turretConfig.AttackInfluenceDistance);
        foreach (var hit in raycatHits)
        {
            var hitTransformPosition = hit.collider.transform.position;
            ShowSparks(hit.point, hit.point - hitTransformPosition);
            var cellHitPosition = CellPositionConverter.WorldToCell(hitTransformPosition);

            dispatcher.Dispatch(MediatorEvents.BULLET_HIT_TARGET_ON_CELL, cellHitPosition);
        }
    }

    private void ShowSparks(Vector3 point, Vector3 direction)
    {
        ViewManager.Instantiate(_turretConfig.BulletSparksPrefab, point, Quaternion.LookRotation(direction));
    }

    private void OnUpdateFiring()
    {
        _bulletTransform.position = _firePoint.position;
        _bulletTransform.rotation = _firePoint.rotation;

        EndFireFramesLeft--;
        if (EndFireFramesLeft <= 0)
        {
            ViewManager.Destroy(_bulletTransform.gameObject);
            _bulletTransform = null;

            UpdateProvider.UpdateAction -= OnUpdateFiring;
        }
    }
}
