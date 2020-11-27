using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class GunTurretMediator : TurretViewWithRotatingHeadMediator
{
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
        AudioManager.Instance.Play(GetSoundId());

        var firePoint = GetFirePoint();
        var bulletGo = ViewManager.Instantiate(TurretModel.TurretConfig.BulletPrefab, firePoint.position, firePoint.rotation);

        var shootTargetPosition = TargetView.AdvanceShootTransform.position;
        shootTargetPosition.y = firePoint.position.y;
        var duration = (shootTargetPosition - firePoint.position).magnitude / TurretModel.TurretConfig.BulletSpeed;
        var moveTween = bulletGo.transform.DOMove(shootTargetPosition, duration);

        var eventParams = new MediatorEventsParams.BulletHitTargetsParams(TurretModel.Damage, TurretModel.TargetUnit);
        moveTween.OnComplete(() => OnBulletHitsTarget(bulletGo, eventParams));
    }

    private SoundId GetSoundId()
    {
        switch(TurretModel.TurretConfig.TurretLevelIndex)
        {
            case 0:
                return SoundId.Gun_1;
            case 1:
                return SoundId.Gun_2;
            default:
                return SoundId.Gun_3;
        }
    }

    private void OnBulletHitsTarget(GameObject bulletGo, MediatorEventsParams.BulletHitTargetsParams param)
    {
        if (TargetView != null)
        {
            ShowSparks(bulletGo.transform.position, bulletGo.transform.position - TargetView.transform.position);

            dispatcher.Dispatch(MediatorEvents.BULLET_HIT_TARGETS, param);
        }

        ViewManager.Destroy(bulletGo);
    }
}
