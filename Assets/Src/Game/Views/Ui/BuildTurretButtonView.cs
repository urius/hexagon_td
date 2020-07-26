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
    [SerializeField] private EventTrigger _eventTrigger;
    [SerializeField] private int _turretTypeId;

    private Color _textFieldDefaultColor;

    public int TurretTypeId => _turretTypeId;

    public void SetEnabled(bool isEnabled)
    {
        _eventTrigger.enabled = isEnabled;
        _button.interactable = isEnabled;
        _costTextField.color = isEnabled ? _textFieldDefaultColor : _disabledTextColor;
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

    protected override void Awake()
    {
        _textFieldDefaultColor = _costTextField.color;

        base.Awake();
    }
}
