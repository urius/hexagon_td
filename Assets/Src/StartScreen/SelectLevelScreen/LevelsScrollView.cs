using System;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class LevelsScrollView : View
{
    public event Action ScrollLeftClicked = delegate { };
    public event Action ScrollRightClicked = delegate { };

    [SerializeField] private RectTransform _content;
    [SerializeField] private Button _scrollLeftButton;
    [SerializeField] private Button _scrollRightButton;

    public RectTransform ContentTransform => _content;

    private void OnEnable()
    {
        _scrollLeftButton.onClick.AddListener(OnScrollLeftClick);
        _scrollRightButton.onClick.AddListener(OnScrollRightClick);
    }

    private void OnDisable()
    {
        _scrollLeftButton.onClick.RemoveAllListeners();
        _scrollRightButton.onClick.RemoveAllListeners();
    }

    private void OnScrollLeftClick()
    {
        ScrollLeftClicked();
    }

    private void OnScrollRightClick()
    {
        ScrollRightClicked();
    }
}
