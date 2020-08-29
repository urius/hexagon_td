using UnityEngine;

public class RocketView : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private GameObject _rocketGo;

    public float ParticleSystemLifetime;

    public void Show()
    {
        ParticleSystemLifetime = _particleSystem.main.startLifetime.constant;

        _rocketGo.SetActive(true);
        _particleSystem.Play();
    }

    public void Hide()
    {
        _rocketGo.SetActive(false);
        _particleSystem.Stop();
    }
}
