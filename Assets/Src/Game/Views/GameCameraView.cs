using System;
using strange.extensions.mediation.impl;
using UnityEngine;

public class GameCameraView : View
{
    public Camera Camera;

    public Vector3 CameraPosition
    {
        get => Camera.transform.position;
        set { Camera.transform.position = value; }
    }

    public Ray GetMouseRay(Vector3 mousePosition)
    {
        return Camera.ScreenPointToRay(mousePosition);
    }
}
