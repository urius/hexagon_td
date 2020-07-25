using System;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenPanelView : View, IPointerDownHandler, IPointerUpHandler
{
    public event Action PointerDown = delegate { };
    public event Action PointerUp = delegate { };

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUp();
    }
}
