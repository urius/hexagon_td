using UnityEngine;

[CreateAssetMenu(menuName = "Configs/UIPrefabsConfig", fileName = "UIPrefabsConfig")]
public class UIPrefabsConfig : ScriptableObject
{
    private const string PrefabsUiFolder = "Prefabs/UI";

    //main screen
    public GameObject MainMenuScreenPrefab;
    public GameObject SelectLevelScreenPrefab;
    public GameObject SelectLevelItemContainerPrefab;
    public GameObject SelectLevelItemPrefab;
    public GameObject SelectLevelItemSelectionPrefab;
    public GameObject HowToPlayScreenPrefab => LoadPrefab($"{PrefabsUiFolder}/HowToPlayScreen");

    //common
    public GameObject ErrorPopupPrefab => LoadPrefab($"{PrefabsUiFolder}/ErrorPopupContainer");

    public GameObject LoadPrefab(string path)
    {
        return Resources.Load<GameObject>(path);
    }
}
