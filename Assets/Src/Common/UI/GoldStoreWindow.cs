using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GoldStoreWindow : MonoBehaviour
{
    public event Action CloseClicked = delegate { };

    [SerializeField] private Button _closeButton;

    [SerializeField] private Text _titleText;
    [SerializeField] private Text _descriptionText;

    [SerializeField] private BuyProductButton _buyButton1;
    [SerializeField] private BuyProductButton _buyButton2;
    [SerializeField] private BuyProductButton _buyButton3;

    private void Awake()
    {
        var iapManager = IAPManager.Instance;
        var buttons = new BuyProductButton[] { _buyButton1, _buyButton2, _buyButton3 };
        for(var i =0; i< buttons.Length; i ++)
        {
            var productId = iapManager.Products[i];
            var goldAmount = int.Parse(productId.Split('_')[1]);
            var product = iapManager.GetProductData(productId);
            buttons[i].Setup(productId, goldAmount, product.metadata.localizedPriceString);
        }
    }

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(OnCloseClicked);
    }

    private void OnCloseClicked()
    {
        CloseClicked();
    }
}
