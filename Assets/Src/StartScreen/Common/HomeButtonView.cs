using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class HomeButtonView : View
{
    public event Action OnClick = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private CanvasGroup _canvasGroup;

    public float Alpha
    {
        get
        {
            return _canvasGroup.alpha;
        }
        set
        {
            _canvasGroup.alpha = value;
        }
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickHandler);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickHandler);
    }

    private void OnClickHandler()
    {
        OnClick();
    }
}
