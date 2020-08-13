using strange.extensions.mediation.impl;
using UnityEngine;

public class BuildTurretView : View
{
    [SerializeField] private Material _material;

    public void SetOkToBuild(bool isokToBuild)
    {
        _material.SetColor("_BaseColor", isokToBuild ? Color.green : Color.red);
    }
}
