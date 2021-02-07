using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldStoreWindow : MonoBehaviour
{
    [SerializeField] private Button _closeButton;

    [SerializeField] private Text _titleText;
    [SerializeField] private Text _descriptionText;

    [SerializeField] private BuyProductButton _buyButton1;
    [SerializeField] private BuyProductButton _buyButton2;
    [SerializeField] private BuyProductButton _buyButton3;

    private void Awake()
    {
        //TODO Setup buttons
    }
}
