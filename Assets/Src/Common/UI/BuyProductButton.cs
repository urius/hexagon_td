using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyProductButton : MonoBehaviour
{
    [SerializeField]
    private string _productId;
    [SerializeField]
    private Button _button;

    void Start()
    {
        _button.onClick.AddListener(OnBuyClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnBuyClick);
    }

    private async void OnBuyClick()
    {
        var result = await new BuyGoldCommand().Execute(_productId);
        if (result)
        {
            gameObject.SetActive(false);
        }
    }
}
