using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Localization", menuName = "Configs/Localization")]
public class LocalizationProvider : ScriptableObject
{
    public static LocalizationProvider Instance { get; private set; }

    [SerializeField]
    private LocalizationGroup[] _localizationGroups;

    private SystemLanguage _debugSystemLanguage;

    private SystemLanguage LanguageId => GetLanguage();

    public string Get(LocalizationGroupId groupId, string itemId)
    {
        var group = _localizationGroups.FirstOrDefault(g => g.LocalizationGroupId == groupId);
        if (group != null)
        {
            var item = group.LocalizationItems.FirstOrDefault(i => i.LocalizationItemId == itemId);
            if (item != null)
            {
                switch (LanguageId)
                {
                    case SystemLanguage.Russian:
                    case SystemLanguage.Belarusian:
                        return ProcessSpecialSymbols(item.Ru);
                    case SystemLanguage.English:
                    default:
                        return ProcessSpecialSymbols(item.En);
                }
            }
        }

        return groupId + ":" + itemId;
    }

    public void SetDebugSystemLanguage(SystemLanguage debugLanguage)
    {
        _debugSystemLanguage = debugLanguage;
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private SystemLanguage GetLanguage()
    {
#if UNITY_EDITOR
        return _debugSystemLanguage;
#else
        return Application.systemLanguage;
#endif
    }

    private string ProcessSpecialSymbols(string original)
    {
        return original.Replace("^", "\n");
    }
}

public enum LocalizationGroupId
{
    TurretName,
    TurretActions,
    BottomPanel,
    GeneralInfoPanel,
    StartScreen,
    TransitionScreen,
    WinPopup,
    LosePopup,
    BootstrapScreen,
    SettingsPopup,
    TutorialScreen,
    ErrorPopup,
    SpecialThanksScreen,
}

[Serializable]
public class LocalizationGroup
{
    public LocalizationGroupId LocalizationGroupId;
    public LocalizationItem[] LocalizationItems;
}

[Serializable]
public class LocalizationItem
{
    public string LocalizationItemId;
    public string En;
    public string Ru;
}
