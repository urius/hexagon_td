using System.Collections.Generic;
using UnityEngine;

public class LaserTurretMediator : TurretViewWithRotationgHeadMediator
{
    const int EndFireFrames = 10;

    private Transform _bulletTransform;
    private Transform _firePoint;
    private int EndFireFramesLeft;

    protected override void Activate()
    {
        base.Activate();

        UpdateFirePoint();

        TurretModel.Fired += OnFired;
    }

    protected override void Deactivate()
    {
        StopFire();
        TurretModel.Fired -= OnFired;

        base.Deactivate();
    }

    protected override void OnTurretUpgraded()
    {
        StopFire();

        base.OnTurretUpgraded();

        UpdateFirePoint();
    }

    private void OnFired()
    {
        if (_bulletTransform == null)
        {
            var bulletGo = ViewManager.Instantiate(TurretModel.TurretConfig.BulletPrefab, _firePoint.position, _firePoint.rotation);
            _bulletTransform = bulletGo.transform;
            var lineRenderer = bulletGo.GetComponentInChildren<LineRenderer>();
            lineRenderer.SetPosition(1, new Vector3(0, 0, TurretConfig.AttackInfluenceDistance));

            UpdateProvider.UpdateAction += OnUpdateFiring;
        }

        EndFireFramesLeft = EndFireFrames;

        CheckCollisions();
    }

    private void UpdateFirePoint()
    {
        _firePoint = TurretView.FirePoints[0].transform;
    }

    private void CheckCollisions()
    {
        var raycatHits = Physics.RaycastAll(_bulletTransform.position, _bulletTransform.forward, TurretConfig.AttackInfluenceDistance);
        var hitList = new List<UnitModel>(raycatHits.Length);
        for (var i = 0; i < raycatHits.Length; i++)
        {
            var hit = raycatHits[i];
            var hitUnit = hit.transform.GetComponent<UnitView>();
            if (hitUnit != null)
            {
                ShowSparks(hit.point, hit.point - hit.transform.position);
                hitList.Add(UnitModelByViews.GetModel(hitUnit));
            }
        }

        if (hitList.Count > 0)
        {
            dispatcher.Dispatch(MediatorEvents.BULLET_HIT_TARGETS, new MediatorEventsParams.BulletHitTargetsParams(TurretModel.Damage, hitList));
        }
    }

    private void OnUpdateFiring()
    {
        _bulletTransform.position = _firePoint.position;
        _bulletTransform.rotation = _firePoint.rotation;

        EndFireFramesLeft--;
        if (EndFireFramesLeft <= 0)
        {
            StopFire();
        }
    }

    private void StopFire()
    {
        if (_bulletTransform != null)
        {
            ViewManager.Destroy(_bulletTransform.gameObject);
            _bulletTransform = null;
        }

        UpdateProvider.UpdateAction -= OnUpdateFiring;
    }
}
