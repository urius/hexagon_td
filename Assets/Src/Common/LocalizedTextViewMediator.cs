using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class LocalizedTextViewMediator : EventMediator
{
    [Inject] public LocalizedTextView LocalizeTextView { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }

    void Start()
    {
        var localizedText = Loc.Get(LocalizeTextView.LocalizationGroupId, LocalizeTextView.LocalizationKey);
        LocalizeTextView.SetText(localizedText);
    }
}
