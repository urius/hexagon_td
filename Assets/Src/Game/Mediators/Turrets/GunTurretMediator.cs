using DG.Tweening;
using UnityEngine;

public class GunTurretMediator : TurretViewWithRotationgHeadMediator
{
    private int _currentFirePointIndex = 0;

    protected override void Activate()
    {
        base.Activate();

        TurretModel.Fired += OnFired;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void Deactivate()
    {
        TurretModel.Fired -= OnFired;

        base.Deactivate();
    }

    private void OnFired()
    {
        var firePoint = GetFirePoint();
        var bulletGo = ViewManager.Instantiate(TurretModel.TurretConfig.BulletPrefab, firePoint.position, firePoint.rotation);

        var shootTargetPosition = TargetView.AdvanceShootTransform.position;
        shootTargetPosition.y = firePoint.position.y;
        var duration = (shootTargetPosition - firePoint.position).magnitude / TurretModel.TurretConfig.BulletSpeed;
        var moveTween = bulletGo.transform.DOMove(shootTargetPosition, duration);

        moveTween.OnComplete(() => OnBulletHitsTarget(bulletGo, TurretModel.TargetUnit));
    }

    private Transform GetFirePoint()
    {
        if (_currentFirePointIndex >= TurretView.FirePoints.Length)
        {
            _currentFirePointIndex = 0;
        }
        return TurretView.FirePoints[_currentFirePointIndex++].transform;
    }

    private void OnBulletHitsTarget(GameObject bulletGo, UnitModel targetModel)
    {
        dispatcher.Dispatch(MediatorEvents.BULLET_HIT_TARGET, targetModel);

        if (TargetView != null)
        {
            //sparks
            ViewManager.Instantiate(
                TurretModel.TurretConfig.BulletSparksPrefab,
                bulletGo.transform.position,
                Quaternion.LookRotation(bulletGo.transform.position - TargetView.transform.position));
        }

        ViewManager.Destroy(bulletGo);
    }
}
