using System;
using UnityEngine;
using UnityEngine.UI;

public class GoldStoreWindow : MonoBehaviour
{
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
        _closeButton.onClick.AddListener(Close);

        var iapManager = IAPManager.Instance;
        var buttons = _buttons;
        for (var i = 0; i < _buttons.Length; i++)
        {
            var productId = iapManager.Products[i];
            var goldAmount = int.Parse(productId.Split('_')[1]);
            var product = iapManager.GetProductData(productId);
            buttons[i].Setup(productId, goldAmount, product.metadata.localizedPriceString, product.metadata.localizedTitle);

            buttons[i].Clicked += OnBuyProductClicked;
        }
    }

    private async void OnBuyProductClicked(string productId)
    {
        _waitOverlay.ToWaitMode();
        var buyGoldResult = await new BuyGoldCommand().Execute(productId);
        if (buyGoldResult.IsSuccess)
        {
            Close();
        }
        else
        {
            _waitOverlay.ToDefaultMode();
            var closeText = LocalizationProvider.Instance.Get(LocalizationGroupId.ErrorPopup, "close");
            var errorText = LocalizationProvider.Instance.Get(LocalizationGroupId.ErrorPopup, "error");
            ErrorPopup.Show(transform.parent as RectTransform, errorText + ": " + buyGoldResult.Error, closeText);
        }
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveListener(Close);

        var buttons = _buttons;
        for (var i = 0; i < buttons.Length; i++)
        {
            buttons[i].Clicked -= OnBuyProductClicked;
        }
    }

    private void Close()
    {
        Destroy(gameObject);
    }
}
