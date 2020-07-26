using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMousePositionProvider
{
    private Vector3 _position;
    public Vector3 Position => _position;

    public void SetPosition(Vector3 position)
    {
        _position = position;
    }
}
