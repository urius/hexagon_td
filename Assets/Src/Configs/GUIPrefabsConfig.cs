using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/GUIPrefabsConfig", fileName = "GUIPrefabsConfig")]
public class GUIPrefabsConfig : ScriptableObject
{
    //game
    public GameObject TurretActionsPrefab;
    public GameObject TurretRadiusPrefab;
    public GameObject TurretSelectionPrefab;
    public GameObject UpgradePSPrefab;
    public GameObject PathLinePrefab;
    public GameObject FlyingTextPrefab;
    public GameObject ExplosionGoalPrefab;
    public GameObject HpBarPrefab;
    public GameObject GeneralInfoPanelPrefab;

    [AssetPath.Attribute(typeof(GameObject))] public string WinPopupPrefabPath;
    public GameObject WinPopupPrefab => LoadPrefab(WinPopupPrefabPath);

    [AssetPath.Attribute(typeof(GameObject))] public string LosePopupPrefabPath;
    public GameObject LosePopupPrefab => LoadPrefab(LosePopupPrefabPath);

    [AssetPath.Attribute(typeof(GameObject))] public string SettingsPopupPrefabPath;
    public GameObject SettingsPopupPrefab => LoadPrefab(SettingsPopupPrefabPath);

    public GameObject LoadPrefab(string path)
    {
        return AssetPath.Load<GameObject>(path);
    }
}
