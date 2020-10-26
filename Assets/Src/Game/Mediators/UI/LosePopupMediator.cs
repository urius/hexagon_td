using strange.extensions.mediation.impl;

public class LosePopupMediator : EventMediator
{
    [Inject] public LosePopup LosePopup { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }

    private void Start()
    {
        LosePopup.SetTitle(Loc.Get(LocalizationGroupId.LosePopup, "title"));
        LosePopup.SetInfo(Loc.Get(LocalizationGroupId.LosePopup, "info"));
    }
}
