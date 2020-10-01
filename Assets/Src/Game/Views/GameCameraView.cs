using System;
using System.Collections;
using strange.extensions.mediation.impl;
using UnityEngine;

public class GameCameraView : View
{
    public Camera Camera;

    private const int CameraShakeDurationFrames = 20;
    private int _cameraShakeRestDuration;
    private Vector3 _cameraOffset = Vector3.zero;
    private Vector3 _cameraPosition = Vector3.zero;

    public Vector3 CameraPosition
    {
        get => _cameraPosition;
        set
        {
            _cameraPosition = value;
            UpdateCameraPosition();
        }
    }

    public Ray GetMouseRay(Vector3 mousePosition)
    {
        return Camera.ScreenPointToRay(mousePosition);
    }

    public void ShakeCamera()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCameraInternal());
    }

    private void UpdateCameraPosition()
    {
        Camera.transform.position = _cameraPosition + _cameraOffset;
    }

    private IEnumerator ShakeCameraInternal()
    {
        _cameraShakeRestDuration = CameraShakeDurationFrames;

        while (_cameraShakeRestDuration > 0)
        {
            _cameraShakeRestDuration--;
            var rnd = 0.5f * UnityEngine.Random.insideUnitCircle * ((float)_cameraShakeRestDuration / CameraShakeDurationFrames);
            _cameraOffset = new Vector3(rnd.x, 0, rnd.y);
            UpdateCameraPosition();

            yield return null;
        }
        _cameraOffset = Vector2.zero;
        UpdateCameraPosition();
    }
}
