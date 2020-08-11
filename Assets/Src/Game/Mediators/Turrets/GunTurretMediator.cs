using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTurretMediator : TurretViewWithRotationgHeadMediator
{
    protected override void OnRegister()
    {
        base.OnRegister();

        TurretModel.Fired += OnFired;
    }

    private void OnFired()
    {
        var firePoint = TurretView.FirePoints[0].transform;
        var bulletGo = ViewManager.Instantiate(TurretModel.TurretConfig.BulletPrefab, firePoint.position, firePoint.rotation);
        var bullet = bulletGo.GetComponent<BulletBase>();
        bullet.Setup(TargetView, TurretModel.TargetUnit, dispatcher, ViewManager);
    }
}
