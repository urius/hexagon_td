using UnityEngine;

[CreateAssetMenu(menuName = "Configs/UIPrefabsConfig", fileName = "UIPrefabsConfig")]
public class UIPrefabsConfig : ScriptableObject
{
    public const string ErrorPopupPrefabPath = PrefabsUiFolder + "/ErrorPopupContainer";

    private const string PrefabsUiFolder = "Prefabs/UI";

    //main screen
    public GameObject MainMenuScreenPrefab;
    public GameObject SelectLevelScreenPrefab;
    public GameObject SelectLevelItemContainerPrefab;
    public GameObject SelectLevelItemPrefab;
    public GameObject SelectLevelItemSelectionPrefab;
    [AssetPath.Attribute(typeof(GameObject))]
    public string HowToPlayScreenPrefabPath;
    public GameObject HowToPlayScreenPrefab => LoadPrefab(HowToPlayScreenPrefabPath);

    public GameObject LoadPrefab(string path)
    {
        return AssetPath.Load<GameObject>(path);
    }
}
