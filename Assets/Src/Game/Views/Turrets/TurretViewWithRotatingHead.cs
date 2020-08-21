using UnityEngine;

public class TurretViewWithRotatingHead : TurretView
{
    [SerializeField] private float _rotationSpeedDegrees;
    [SerializeField] private Transform _head;

    private float _rotationSpeedRadians;
    private Transform _targetTransform;

    public bool IsLookOnTarget { get; private set; }
    public Quaternion TurretRotation { get => _head.rotation; set { _head.transform.rotation = value; } }

    protected override void Awake()
    {
        base.Awake();

        _rotationSpeedRadians = _rotationSpeedDegrees * Mathf.Deg2Rad;
    }

    public void SetTargetTransform(Transform target)
    {
        _targetTransform = target;
        IsLookOnTarget = false;
    }

    private void Update()
    {
        if (_targetTransform != null)
        {
            var targetRotation = GetTargetRotation(_targetTransform);
            _head.rotation = Quaternion.RotateTowards(_head.rotation, targetRotation, _rotationSpeedDegrees);
            IsLookOnTarget = Quaternion.Angle(_head.rotation, targetRotation) < _rotationSpeedRadians;
        }
    }

    private Quaternion GetTargetRotation(Transform targetTransform)
    {
        var lookVector = targetTransform.position - _head.position;
        lookVector.y = 0;
        return Quaternion.LookRotation(lookVector, Vector3.up);
    }
}
