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
    [SerializeField] private Text _upgradeButtonText;
    [SerializeField] private Animation _animation;
    [SerializeField] private RectTransform _leftBound;
    [SerializeField] private RectTransform _rightBound;

    public Animation Animation => _animation;
    public RectTransform LeftBound => _leftBound;
    public RectTransform RightBound => _rightBound;

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
