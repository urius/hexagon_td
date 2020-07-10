using System;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnBaseCellView : CellView
{
    public event Action PLatformBottomPointReached = delegate { };

    [SerializeField] private Animation _spawnAnimation;
    private TaskCompletionSource<bool> _animationTsc;

    [SerializeField] private Transform _spawnPoint;
    public Transform SpawnPoint => _spawnPoint;

    public void OnPLatformBottomPointReached()
    {
        PLatformBottomPointReached();
    }

    public void OnPLatformTopPointReached()
    {
        _animationTsc.TrySetResult(true);
    }

    public Task PlaySpawnAnimationAsync()
    {
        _animationTsc = new TaskCompletionSource<bool>();
        _spawnAnimation.Play();

        return _animationTsc.Task;
    }
}
