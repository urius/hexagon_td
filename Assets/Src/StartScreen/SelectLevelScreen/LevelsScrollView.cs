using System;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class LevelsScrollView : View
{
    public event Action ScrollLeftClicked = delegate { };
    public event Action ScrollRightClicked = delegate { };
    public event Action DragEnded = delegate { };

    [SerializeField] private RectTransform _content;
    [SerializeField] private Button _scrollLeftButton;
    [SerializeField] private Button _scrollRightButton;
    [SerializeField] private ScrollRect _scrollRect;

    public RectTransform ContentTransform => _content;

    public void Event_OnEndDrag()
    {
        DragEnded();
    }

    public void StopMovement()
    {
        _scrollRect.StopMovement();
    }

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
