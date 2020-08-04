using strange.extensions.mediation.impl;
using UnityEngine;

public class TurretView : View
{
    [SerializeField] private Transform[] _firePoints;
    [SerializeField] private ParticleSystem _particles;

    public Transform[] FirePoints => _firePoints;

    public void Fire(int firePointIndex = 0)
    {
        if (_particles == null)
        {
            return;
        }
        _particles.transform.position = _firePoints[firePointIndex].transform.position;
        _particles.transform.rotation = _firePoints[firePointIndex].transform.rotation;
        _particles.Play();
    }
}
