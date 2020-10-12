using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedButtonViewMediator : ButtonViewMediator
{
    [Inject] public LocalizedButtonView LocalizedButtonView { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }

    void Start()
    {
        var localizedText = Loc.Get(LocalizedButtonView.LocalizationGroupId, LocalizedButtonView.LocalizationKey);
        LocalizedButtonView.SetText(localizedText);
    }
}
