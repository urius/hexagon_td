using System;
using System.Collections.Generic;
using UnityEngine;

public class RocketTurretMediator : TurretViewWithRotatingHeadMediator
{
    private readonly List<RocketFlyData> _rockets = new List<RocketFlyData>();

    private int _rocketIndexBuf;

    public RocketTurretMediator()
    {
    }

    protected override void Activate()
    {
        base.Activate();

        TurretModel.Fired += OnFired;
    }

    protected override void Deactivate()
    {
        TurretModel.Fired -= OnFired;

        foreach (var rocket in _rockets)
        {
            ViewManager.Destroy(rocket.View.gameObject);
        }
        _rockets.Clear();

        base.Deactivate();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if(!TargetIsLocked && TargetView != null)
        {
            TargetIsLocked = true;
            dispatcher.Dispatch(MediatorEvents.TURRET_TARGET_LOCKED, TurretModel);
        } 

        _rocketIndexBuf = 0;
        while (_rocketIndexBuf < _rockets.Count)
        {
            if (ProcessRocket(_rockets[_rocketIndexBuf]))
            {
                _rocketIndexBuf++;
            }
            else
            {
                _rockets.RemoveAt(_rocketIndexBuf);
            }
        }
    }

    private bool ProcessRocket(RocketFlyData rocketFlyData)
    {
        rocketFlyData.AdvanceTime(Time.deltaTime);

        if (rocketFlyData.IsDestroyed && rocketFlyData.FlySecondsLeft < -rocketFlyData.View.ParticleSystemLifetime)
        {
            ViewManager.Destroy(rocketFlyData.View.gameObject);
            return false;
        }
        else if (!rocketFlyData.IsDestroyed && rocketFlyData.FlySecondsLeft > 0 && rocketFlyData.Transform.position.y > 0)
        {
            if (!rocketFlyData.IsFlyingStraight && rocketFlyData.TargetView != null)
            {
                var targetRotation = Quaternion.LookRotation(rocketFlyData.TargetTransform.position - rocketFlyData.Transform.position);
                rocketFlyData.Transform.rotation = Quaternion.RotateTowards(rocketFlyData.Transform.rotation, targetRotation, rocketFlyData.RotationSpeed);

                if ((rocketFlyData.Transform.position - rocketFlyData.TargetTransform.position).sqrMagnitude < 5)
                {
                    rocketFlyData.IsDestroyed = true;
                    rocketFlyData.View.Hide();

                    if (UnitModelByViews.TryGetModel(rocketFlyData.TargetView, out var targetUnitModel))
                    {
                        ShowSparks(rocketFlyData.Transform.position, Vector3.up);

                        dispatcher.Dispatch(MediatorEvents.BULLET_HIT_TARGETS, new MediatorEventsParams.BulletHitTargetsParams(
                            TurretConfig.Damage, targetUnitModel));
                    }
                }
            }

            rocketFlyData.Transform.position += rocketFlyData.Transform.forward * rocketFlyData.Speed * Time.deltaTime;
        }
        else if (!rocketFlyData.IsDestroyed)
        {
            rocketFlyData.IsDestroyed = true;
            rocketFlyData.View.Hide();

            ShowSparks(rocketFlyData.Transform.position, Vector3.up);
        }

        return true;
    }

    private void OnFired()
    {
        var firePoint = GetFirePoint();
        var rocketGo = ViewManager.Instantiate(TurretConfig.BulletPrefab, firePoint.position, firePoint.rotation);
        var rocketView = rocketGo.GetComponent<RocketView>();

        rocketView.Show();
        _rockets.Add(new RocketFlyData(rocketView, TargetView, 3, TurretConfig));
    }
}

public class RocketFlyData
{
    public readonly RocketView View;
    public readonly Transform Transform;
    public readonly UnitView TargetView;
    public readonly Transform TargetTransform;
    public readonly int Speed;
    public readonly int RotationSpeed = 25;

    public float FlySecondsLeft;
    public float FlyStraightSecondsLeft;
    public bool IsDestroyed;

    public RocketFlyData(
        RocketView view,
        UnitView targetView,
        float FlySeconds,
        TurretConfig turretConfig)
    {
        View = view;
        TargetView = targetView;
        TargetTransform = targetView.AdvanceShootTransform;
        Transform = view.transform;
        FlySecondsLeft = FlySeconds;
        Speed = turretConfig.BulletSpeed;

        IsDestroyed = false;
        FlyStraightSecondsLeft = 0.1f;
    }

    public bool IsFlyingStraight => FlyStraightSecondsLeft > 0;

    public void AdvanceTime(float deltaTime)
    {
        FlySecondsLeft -= deltaTime;
        FlyStraightSecondsLeft -= deltaTime;
    }
}
