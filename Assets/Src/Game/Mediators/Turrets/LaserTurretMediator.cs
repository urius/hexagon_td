using System.Collections.Generic;
using UnityEngine;

public class LaserTurretMediator : TurretViewWithRotatingHeadMediator
{
    const int EndFireFrames = 15;

    private Transform _bulletTransform;
    private Transform _firePoint;
    private int EndFireFramesLeft;
    private SoundId _fireSoundId;
    private float _fireSoundLength;
    private float _lastShotTime = -1;
    private AudioSource _audioSource;

    protected override void Activate()
    {
        base.Activate();

        UpdateFirePoint();

        _fireSoundId = GetSoundId();
        _fireSoundLength = AudioManager.Instance.GetSoundLength(_fireSoundId);
        if (_audioSource == null) _audioSource = AudioManager.Instance.CreateAudioSource();
        _audioSource.loop = true;

        TurretModel.Fired += OnFired;
        LevelModel.PauseModeChanged += OnPauseModeChanged;
    }

    protected override void Deactivate()
    {
        StopFire();
        LevelModel.PauseModeChanged -= OnPauseModeChanged;
        TurretModel.Fired -= OnFired;

        AudioManager.Instance.RemoveAudioSource(_audioSource);
        _audioSource = null;

        base.Deactivate();
    }

    protected override void OnTurretUpgraded()
    {
        StopFire();

        base.OnTurretUpgraded();

        UpdateFirePoint();
    }

    private bool IsFireSoundFinished => !_audioSource.isPlaying;// Time.realtimeSinceStartup - _lastShotTime >= _fireSoundLength;

    private void OnPauseModeChanged()
    {
        if (LevelModel.IsPaused)
        {
            _audioSource.Stop();
        }
    }

    private void OnFired()
    {
        if (_bulletTransform == null)
        {
            var bulletGo = ViewManager.Instantiate(TurretModel.TurretConfig.BulletPrefab, _firePoint.position, _firePoint.rotation);
            _bulletTransform = bulletGo.transform;
            var lineRenderer = bulletGo.GetComponentInChildren<LineRenderer>();
            lineRenderer.SetPosition(1, new Vector3(0, 0, TurretConfig.AttackInfluenceDistance));

            UpdateProvider.UpdateAction += OnUpdateFiring;
        }

        EndFireFramesLeft = EndFireFrames;

        CheckCollisions();

        PlayFireSoundIfNeeded();
    }

    private void PlayFireSoundIfNeeded()
    {
        if (IsFireSoundFinished)
        {
            AudioManager.Instance.PlayOnSource(_audioSource, _fireSoundId);
            _lastShotTime = Time.realtimeSinceStartup;
        }
    }

    private void UpdateFirePoint()
    {
        _firePoint = TurretView.FirePoints[0].transform;
    }

    private void CheckCollisions()
    {
        var raycatHits = Physics.RaycastAll(_bulletTransform.position, _bulletTransform.forward, TurretConfig.AttackInfluenceDistance);
        var hitList = new List<UnitModel>(raycatHits.Length);
        for (var i = 0; i < raycatHits.Length; i++)
        {
            var hit = raycatHits[i];
            var hitUnit = hit.transform.GetComponent<UnitView>();
            if (hitUnit != null)
            {
                ShowSparks(hit.point, hit.point - hit.transform.position);
                hitList.Add(UnitModelByViews.GetModel(hitUnit));
            }
        }

        if (hitList.Count > 0)
        {
            dispatcher.Dispatch(MediatorEvents.BULLET_HIT_TARGETS, new MediatorEventsParams.BulletHitTargetsParams(TurretModel.Damage, hitList));
        }
    }

    private void OnUpdateFiring()
    {
        _bulletTransform.position = _firePoint.position;
        _bulletTransform.rotation = _firePoint.rotation;

        EndFireFramesLeft--;
        if (EndFireFramesLeft < 0)
        {
            StopFire();
        }
    }

    private void StopFire()
    {
        if (_bulletTransform != null)
        {
            ViewManager.Destroy(_bulletTransform.gameObject);
            _bulletTransform = null;

            _audioSource.Stop();
        }

        UpdateProvider.UpdateAction -= OnUpdateFiring;
    }

    private SoundId GetSoundId()
    {
        switch (TurretModel.TurretConfig.TurretLevelIndex)
        {
            case 0:
                return SoundId.Laser_1;
            case 1:
                return SoundId.Laser_2;
            default:
                return SoundId.Laser_3;
        }
    }
}
