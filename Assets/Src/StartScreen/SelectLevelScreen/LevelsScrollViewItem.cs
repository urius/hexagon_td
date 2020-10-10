using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsScrollViewItem : MonoBehaviour
{
    public event Action OnClick = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Image _bgImage;
    [SerializeField] private Text _text;
    [SerializeField] private Sprite _passedSprite;
    [SerializeField] private Sprite _notPassedSprite;
    [SerializeField] private Image[] _stars;
    [SerializeField] private Color _starAchievedColor = Color.yellow;
    private Color _starNotAchievedColor = Color.black;
    [SerializeField] private Image _lockImage;
    [SerializeField] private Color _defaultBgColor;
    [SerializeField] private Color _lockedBgColor;

    private void OnButtonClick()
    {
        OnClick();
    }

    public void SetLevelNum(int num)
    {
        _text.text = num.ToString();
    }

    public void SetPassedMode(bool isPassed)
    {
        _bgImage.sprite = isPassed ? _passedSprite : _notPassedSprite;
        for (var i = 0; i < _stars.Length; i++)
        {
            _stars[i].gameObject.SetActive(isPassed);
        }
    }

    public void SetupStars(int reachedStarsNum)
    {
        for (var i = 0; i < _stars.Length; i++)
        {
            _stars[i].color = (i <= reachedStarsNum - 1) ? _starAchievedColor : _starNotAchievedColor;
        }
    }

    public void SetLocked(bool isLocked)
    {
        _lockImage.gameObject.SetActive(isLocked);
        _bgImage.color = isLocked ? _lockedBgColor : _defaultBgColor;
    }

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClick);
    }
}
