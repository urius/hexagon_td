using System;
using UnityEngine;

public class BGAnimatedView : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRectTransform;

    private RectTransform _bgRectTransform;
    private float _bgHorizontalBound;
    private float _moveSpeed;
    private int _delayFramesLeft;

    private void Awake()
    {
        _bgRectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _bgHorizontalBound = Math.Max(_bgRectTransform.sizeDelta.x - _canvasRectTransform.sizeDelta.x, 0) / 2;

        _moveSpeed = _bgHorizontalBound > 0 ? 0.1f : 0f;
    }

    void FixedUpdate()
    {
        if (_delayFramesLeft > 0)
        {
            _delayFramesLeft--;
            return;
        }

        SetBgAnchoredXPosition(_bgRectTransform.anchoredPosition.x + _moveSpeed);

        if (_bgRectTransform.anchoredPosition.x > _bgHorizontalBound)
        {
            SetBgAnchoredXPosition(_bgHorizontalBound);
            _moveSpeed *= -1;
            _delayFramesLeft = 60;
            return;
        }
        else if (_bgRectTransform.anchoredPosition.x < -_bgHorizontalBound)
        {
            SetBgAnchoredXPosition(-_bgHorizontalBound);
            _moveSpeed *= -1;
            _delayFramesLeft = 60;
            return;
        }
    }

    private void SetBgAnchoredXPosition(float x)
    {
        _bgRectTransform.anchoredPosition = new Vector2(x, _bgRectTransform.anchoredPosition.y);
    }
}
