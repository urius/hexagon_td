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

    [SerializeField] private WaitPurchaseOverlay _waitOverlay;

    [SerializeField] private BuyProductButton _buyButton1;
    [SerializeField] private BuyProductButton _buyButton2;
    [SerializeField] private BuyProductButton _buyButton3;

    private BuyProductButton[] _buttons => new BuyProductButton[] { _buyButton1, _buyButton2, _buyButton3 };

    private void Awake()
    {
        var iapManager = IAPManager.Instance;
        var buttons = _buttons;
        for (var i = 0; i < _buttons.Length; i++)
        {
            var productId = iapManager.Products[i];
            var goldAmount = int.Parse(productId.Split('_')[1]);
            var product = iapManager.GetProductData(productId);
            buttons[i].Setup(productId, goldAmount, product.metadata.localizedPriceString);

            buttons[i].Clicked += OnBuyProductClicked;
        }
    }

    private async void OnBuyProductClicked(string productId)
    {
        _waitOverlay.ToWaitMode();
        var isBuySuccess = await new BuyGoldCommand().Execute(productId);
        if (isBuySuccess)
        {
            CloseClicked();
        }
        else
        {
            _waitOverlay.ToDefaultMode();
        }
    }

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(OnCloseClicked);
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveListener(OnCloseClicked);
        var buttons = _buttons;
        for (var i = 0; i < buttons.Length; i++)
        {
            buttons[i].Clicked -= OnBuyProductClicked;
        }
    }

    private void OnCloseClicked()
    {
        CloseClicked();
    }
}
