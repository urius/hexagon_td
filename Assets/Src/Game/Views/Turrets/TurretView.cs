using System;
using System.Collections;
using strange.extensions.mediation.impl;
using UnityEngine;

public class TurretView : View
{
    public event Action ParticlesBurst = delegate { };

    [SerializeField] private Transform[] _firePoints;
    [SerializeField] private ParticleSystem _particles;

    public Transform[] FirePoints => _firePoints;
    public ParticleSystem Particles => _particles;
    public bool IsFiring { get; private set; }

    private Coroutine _fireCoroutine;

    public void StartFire()
    {
        if (!IsFiring)
        {
            IsFiring = true;
            _particles?.Play();

            _fireCoroutine = StartCoroutine(FireCoroutine(_particles.main.duration));
        }
    }

    public void StopFire()
    {
        if (IsFiring)
        {
            IsFiring = false;
            _particles?.Stop();

            if (_fireCoroutine != null)
            {
                StopCoroutine(_fireCoroutine);
                _fireCoroutine = null;
            }
        }
    }

    override protected void OnDestroy()
    {
        StopAllCoroutines();

        base.OnDestroy();
    }

    private IEnumerator FireCoroutine(float durationSec)
    {
        while (true)
        {
            ParticlesBurst();

            yield return new WaitForSeconds(durationSec);
        }
    }
}
