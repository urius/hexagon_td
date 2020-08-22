using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class UnitView : View
{
    [SerializeField]
    private Transform _advanceShootTransform;

    public Transform AdvanceShootTransform => _advanceShootTransform;
}
