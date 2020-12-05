using UnityEngine;

public class TurretViewWithRotatingHead : TurretView
{
    [SerializeField] private Transform _head;

    public Vector3 HeadPosition => _head.position;
    public Quaternion HeadRotation { get => _head.rotation; set { _head.rotation = value; } }
}
