using strange.extensions.mediation.impl;
using UnityEngine;

public class TurretView : View
{
    [SerializeField] private Transform[] _firePoints;
    [SerializeField] private ParticleSystem _particles;

    public Transform[] FirePoints => _firePoints;
    public ParticleSystem Particles => _particles;

    private bool _isFiring = false;

    public void StartFire()
    {
        if (!_isFiring)
        {
            _isFiring = true;
            _particles?.Play();
        }
    }

    public void StopFire()
    {
        if (_isFiring)
        {
            _isFiring = false;
            _particles?.Stop();
        }
    }
}
