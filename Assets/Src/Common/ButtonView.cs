using System;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class ButtonView : View
{
    public event Action OnClick = delegate { };

    [SerializeField] private Text _text;

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void EventOnClick()
    {
        OnClick();
    }
}
