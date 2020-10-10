using UnityEngine;

[CreateAssetMenu(menuName = "Configs/UIPrefabsConfig", fileName = "UIPrefabsConfig")]
public class UIPrefabsConfig : ScriptableObject
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

    //main screen
    public GameObject SelectLevelItemContainerPrefab;
    public GameObject SelectLevelItemPrefab;
    public GameObject SelectLevelItemSelectionPrefab;
}
