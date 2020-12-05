using System;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildTurretButtonView : View
{
    public event Action ButtonPointerDown = delegate { };
    public event Action ButtonPointerUp = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Text _costTextField;
    [SerializeField] private Color _disabledTextColor;
    [SerializeField] private Color _textFieldDefaultColor;
    [SerializeField] private EventTrigger _eventTrigger;
    [SerializeField] private int _turretTypeId;


    public int TurretTypeId => _turretTypeId;

    public void SetEnabled(bool isEnabled)
    {
        _eventTrigger.enabled = isEnabled;
        _button.interactable = isEnabled;
        _costTextField.color = isEnabled ? _textFieldDefaultColor : _disabledTextColor;
    }

    public void SetCost(int price)
    {
        _costTextField.text = $"{price}$";
    }

    public void OnButtonPointerDown()
    {
        Debug.Log("OnButtonPointerDown");
        ButtonPointerDown();
    }

    public void OnButtonPointerUp()
    {
        ButtonPointerUp();
    }
}
