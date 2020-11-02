using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class MenuSceneCanvasView : View
{
    [SerializeField] private RectTransform _rootTransform;
    public RectTransform RootTransform => _rootTransform;
}
