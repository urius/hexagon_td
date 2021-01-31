using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Localization", menuName = "Configs/Localization")]
public class LocalizationProvider : ScriptableObject
{
    public static LocalizationProvider Instance { get; private set; }

    [SerializeField]
    private LocalizationGroup[] _localizationGroups;

    private string _languageId = "en";

    public string Get(LocalizationGroupId groupId, string itemId)
    {
        var group = _localizationGroups.FirstOrDefault(g => g.LocalizationGroupId == groupId);
        if (group != null)
        {
            var item = group.LocalizationItems.FirstOrDefault(i => i.LocalizationItemId == itemId);
            if (item != null)
            {
                switch (_languageId)
                {
                    case "en":
                        return ProcessSpecialSymbols(item.En);
                    default:
                        return "unsupported lang: " + _languageId;
                }
            }
        }

        return groupId + ":" + itemId;
    }

    private void OnEnable()
    {
        Instance = this;
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
}
