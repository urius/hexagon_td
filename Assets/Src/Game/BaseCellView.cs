using System;
using UnityEngine;

public class BaseCellView : CellView
{
    public event Action PLatformBottomPointReached = delegate { };
    public event Action PLatformTopPointReached = delegate { };

    [SerializeField] private Transform _spawnPoint;
    public Transform SpawnPoint => _spawnPoint;

    public void OnPLatformBottomPointReached()
    {
        PLatformBottomPointReached();
    }

    public void OnPLatformTopPointReached()
    {
        PLatformTopPointReached();
    }
}
