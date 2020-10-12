using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedButtonView : ButtonView
{
    [SerializeField] private LocalizationGroupId _localizationGroupId;
    [SerializeField] private string _localizationKey;

    public LocalizationGroupId LocalizationGroupId => _localizationGroupId;
    public string LocalizationKey => _localizationKey;
}
