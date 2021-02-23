using System;
using UnityEngine;
using UnityEngine.UI;

public class TurretActionsView : MonoBehaviour
{
    public event Action SellClicked = delegate { };
    public event Action UpgradeClicked = delegate { };

    [SerializeField] private Text _turretText;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Text _sellButtonText;
    [SerializeField] private Text _sellPriceText;
    [SerializeField] private Text _upgradeButtonText;
    [SerializeField] private Text _upgradePriceText;
    [SerializeField] private Animation _animation;
    [SerializeField] private RectTransform _leftBound;
    [SerializeField] private RectTransform _rightBound;

    public Animation Animation => _animation;
    public RectTransform LeftBound => _leftBound;
    public RectTransform RightBound => _rightBound;

    public void SetTurretInfoTexts(string turretName, string turretLevel)
    {
        _turretText.text = turretName + "\n" + turretLevel;
    }

    public void SetSellTexts(string sellTxt, string sellPriceTxt)
    {
        _sellButtonText.text = sellTxt;
        _sellPriceText.text = sellPriceTxt;
    }

    public void SetUpgradeButtonEnabled(bool isEnabled)
    {
        _upgradeButton.interactable = isEnabled;
        _upgradePriceText.gameObject.SetActive(isEnabled);
        _upgradeButtonText.gameObject.SetActive(isEnabled);
    }

    public void SetUpgradeTexts(string upgradeTxt, string upgradePriceTxt)
    {
        _upgradeButtonText.text = upgradeTxt;
        _upgradePriceText.text = upgradePriceTxt;
    }

    private void OnEnable()
    {
        _sellButton.onClick.AddListener(OnSellClick);
        _upgradeButton.onClick.AddListener(OnUpgradeClick);
    }

    private void OnDisable()
    {
        _sellButton.onClick.RemoveListener(OnSellClick);
        _upgradeButton.onClick.RemoveListener(OnUpgradeClick);
    }

    private void OnSellClick()
    {
        SellClicked();
    }

    private void OnUpgradeClick()
    {
        UpgradeClicked();
    }
}
