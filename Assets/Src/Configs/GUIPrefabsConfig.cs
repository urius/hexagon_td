using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/GUIPrefabsConfig", fileName = "GUIPrefabsConfig")]
public class GUIPrefabsConfig : ScriptableObject
{
    private const string PrefabsUiFolder = "Prefabs/UI";

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
    public GameObject WinPopupPrefab;
    public GameObject LosePopupPrefab;
    public GameObject SettingsPopupPrefab => LoadPrefab($"{PrefabsUiFolder}/SettingsPopup");

    public GameObject LoadPrefab(string path)
    {
        return Resources.Load<GameObject>(path);
    }
}
