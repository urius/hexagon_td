using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Localization", menuName = "Configs/Localization")]
public class LocalizationProvider : ScriptableObject
{
    [SerializeField]
    private LocalizationGroup[] _localizationGroups;

    private string _languageId = "en";

    public string GetLocalization(LocalizationGroupId groupId, string itemId)
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
                        return item.En;
                    default:
                        return "unsupported lang: " + _languageId;
                }
            }
        }

        return groupId + ":" + itemId;
    }
}

public enum LocalizationGroupId
{
    TurretName,
    TurretActions,
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
