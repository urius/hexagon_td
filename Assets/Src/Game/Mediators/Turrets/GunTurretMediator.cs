using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class GunTurretMediator : TurretViewWithRotationgHeadMediator
{
    private int _currentFirePointIndex = 0;

    protected override void Activate()
    {
        base.Activate();

        TurretModel.Fired += OnFired;
    }

    protected override void Deactivate()
    {
        TurretModel.Fired -= OnFired;

        base.Deactivate();
    }

    protected override Transform GetTransformForTargeting([CanBeNull] UnitView unitView)
    {
        return unitView?.AdvanceShootTransform;
    }

    private void OnFired()
    {
        var firePoint = GetFirePoint();
        var bulletGo = ViewManager.Instantiate(TurretModel.TurretConfig.BulletPrefab, firePoint.position, firePoint.rotation);

        var shootTargetPosition = TargetView.AdvanceShootTransform.position;
        shootTargetPosition.y = firePoint.position.y;
        var duration = (shootTargetPosition - firePoint.position).magnitude / TurretModel.TurretConfig.BulletSpeed;
        var moveTween = bulletGo.transform.DOMove(shootTargetPosition, duration);

        var eventParams = new MediatorEventsParams.BulletHitTargetsParams(TurretModel.Damage, TurretModel.TargetUnit);
        moveTween.OnComplete(() => OnBulletHitsTarget(bulletGo, eventParams));
    }

    private Transform GetFirePoint()
    {
        if (_currentFirePointIndex >= TurretView.FirePoints.Length)
        {
            _currentFirePointIndex = 0;
        }
        return TurretView.FirePoints[_currentFirePointIndex++].transform;
    }

    private void OnBulletHitsTarget(GameObject bulletGo, MediatorEventsParams.BulletHitTargetsParams param)
    {
        if (TargetView != null)
        {
            //sparks
            ViewManager.Instantiate(
                TurretModel.TurretConfig.BulletSparksPrefab,
                bulletGo.transform.position,
                Quaternion.LookRotation(bulletGo.transform.position - TargetView.transform.position));

            dispatcher.Dispatch(MediatorEvents.BULLET_HIT_TARGETS, param);
        }

        ViewManager.Destroy(bulletGo);
    }
}
