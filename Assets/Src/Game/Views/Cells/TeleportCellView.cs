using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCellView : CellView
{
    [SerializeField] private ParticleSystem _particleSystemOut;
    [SerializeField] private ParticleSystem _particleSystemIn;

    public void PlayTeleportOutAnimation()
    {
        _particleSystemOut.Clear();
        _particleSystemOut.Play();
    }

    public void PlayTeleportInAnimation()
    {
        _particleSystemIn.Clear();
        _particleSystemIn.Play();
    }
}
