using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedTextView : View
{
    [SerializeField] private Text _text;
    [SerializeField] private LocalizationGroupId _localizationGroupId;
    [SerializeField] private string _localizationKey;

    public LocalizationGroupId LocalizationGroupId => _localizationGroupId;
    public string LocalizationKey => _localizationKey;

    public void SetText(string text)
    {
        _text.text = text;
    }
}
