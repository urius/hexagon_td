using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyProductButton : MonoBehaviour
{
    public event Action<string> Clicked = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Text _goldAmount;
    [SerializeField] private Text _description;
    [SerializeField] private Text _price;

    private string _productId;

    public void Start()
    {
        _button.onClick.AddListener(OnBuyClick);
    }

    public void Setup(string productId, int goldAmount, string priceText, string description)
    {
        _productId = productId;
        _goldAmount.text = "+" + goldAmount.ToSpaceSeparatedAmount();
        _price.text = priceText;
        _description.text = description;
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnBuyClick);
    }

    private void OnBuyClick()
    {
        Clicked(_productId);
    }
}
