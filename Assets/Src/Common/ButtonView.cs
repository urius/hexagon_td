using System;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class ButtonView : View
{
    public event Action OnClick = delegate { };

    [SerializeField] private Text _text;
    [SerializeField] private SoundId _sound;

    private Button _button;

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void EventOnClick()
    {
        OnClick();
        AudioManager.Instance.Play(_sound); //Little hack
    }

    override protected void Awake()
    {
        base.Awake();

        _button = GetComponent<Button>();
        _button.onClick.AddListener(EventOnClick);
    }

    override protected void OnDestroy()
    {
        _button.onClick.RemoveListener(EventOnClick);

        base.OnDestroy();
    }
}
