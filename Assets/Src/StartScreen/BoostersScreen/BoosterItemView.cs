using System;
using UnityEngine;
using UnityEngine.UI;

public class BoosterItemView : MonoBehaviour
{
    public event Action CLicked = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Image _bgImg;
    [SerializeField] private Color _isCheckedBgColor;
    [SerializeField] private Color _isNotCheckedBgColor;
    [SerializeField] private Image _iconImg;
    [SerializeField] private Text _titleText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private Text _priceText;
    [SerializeField] private Image _checkboxImg;
    [SerializeField] private Sprite _isCheckedSprite;
    [SerializeField] private Sprite _isNotCheckedSprite;

    public void Setup(Sprite iconSprite, string titleText, string descriptionText, string priceText)
    {
        _iconImg.sprite = iconSprite;

        _titleText.text = titleText;
        _descriptionText.text = descriptionText;
        _priceText.text = priceText;
    }

    public void SetCheckedState(bool isChecked)
    {
        _bgImg.color = isChecked ? _isCheckedBgColor : _isNotCheckedBgColor;
        _checkboxImg.sprite = isChecked ? _isCheckedSprite : _isNotCheckedSprite;

        var color = _iconImg.color;
        color.a = isChecked ? 1f : 0.5f;
        _iconImg.color = color;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        CLicked();
    }
}
