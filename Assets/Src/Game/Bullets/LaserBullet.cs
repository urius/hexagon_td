using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class LaserBullet : BulletBase
{
    private Vector3 _targetPosition;
    private UnitModel _targetModel;
    private IEventDispatcher _dispatcher;
    private IViewManager _viewManager;

    private float _lastSqrDistance;
    private float _speed = 40;

    public override void Setup(UnitView targetView, UnitModel targetModel, IEventDispatcher dispatcher, IViewManager viewManager)
    {
        _targetPosition = targetView.transform.position;
        _targetModel = targetModel;
        _dispatcher = dispatcher;
        _viewManager = viewManager;

        _lastSqrDistance = (_targetPosition - transform.position).sqrMagnitude;
    }

    private void FixedUpdate()
    {
        var newPosition = transform.position + transform.TransformVector(new Vector3(0, 0, _speed * Time.fixedDeltaTime));
        var newSqrDistance = (_targetPosition - newPosition).sqrMagnitude;
        if (newSqrDistance > _lastSqrDistance)
        {            
            _dispatcher.Dispatch(MediatorEvents.BULLET_HIT_TARGET, _targetModel);

            _viewManager.Destroy(gameObject);
        }
        else
        {
            _lastSqrDistance = newSqrDistance;
            transform.position = newPosition;
        }
    }
}
