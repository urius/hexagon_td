using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class WaveTextView : View
{
    [SerializeField] private Text _text;
    [SerializeField] private GameObject _bg;

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        _bg.SetActive(isActive);
    }
}
