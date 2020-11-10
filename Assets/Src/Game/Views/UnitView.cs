using System;
using strange.extensions.mediation.impl;
using UnityEngine;

public class UnitView : View
{
    [SerializeField]
    private Transform _advanceShootTransform;
    [SerializeField]
    private Transform _unitTransform;
    [SerializeField]
    private Vector3 _rotationMaxSpeed;

    private Vector3 _rotationDirection;

    public Transform AdvanceShootTransform => _advanceShootTransform;

    override protected void Awake()
    {
        base.Awake();

        _rotationDirection = new Vector3(GetRandom(_rotationMaxSpeed.x), GetRandom(_rotationMaxSpeed.y), GetRandom(_rotationMaxSpeed.z));
    }

    private float GetRandom(float amplitude)
    {
        return UnityEngine.Random.Range(-amplitude / 2, amplitude / 2);
    }

    public void UpdateDelegate()
    {
        _unitTransform.Rotate(_rotationDirection);
    }
}
