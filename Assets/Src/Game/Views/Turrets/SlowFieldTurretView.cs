using UnityEngine;

public class SlowFieldTurretView : TurretView
{
    [SerializeField] private Color _particlesColor;

    protected override void Awake()
    {
        base.Awake();

        var module = Particles.main;
        module.startColor = _particlesColor;
    }

    public void SetParticlesLifetime(float t)
    {
        var module = Particles.main;
        module.startLifetime = t;
    }
}
