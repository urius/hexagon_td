using UnityEngine;

[CreateAssetMenu(menuName = "Configs/UIPrefabsConfig", fileName = "UIPrefabsConfig")]
public class UIPrefabsConfig : ScriptableObject
{
    public static UIPrefabsConfig Instance { get; private set; }
    private void OnEnable()
    {
        Instance = this;
    }

    //main screen
    public GameObject MainMenuScreenPrefab;
    public GameObject SelectLevelScreenPrefab;
    public GameObject SelectLevelItemContainerPrefab;
    public GameObject SelectLevelItemPrefab;
    public GameObject SelectLevelItemSelectionPrefab;

    [AssetPath.Attribute(typeof(GameObject))] public string ErrorPopupPrefabPath;
    public GameObject ErrorPopupPrefab => LoadPrefab(ErrorPopupPrefabPath);
    [AssetPath.Attribute(typeof(GameObject))] public string HowToPlayScreenPrefabPath;
    public GameObject HowToPlayScreenPrefab => LoadPrefab(HowToPlayScreenPrefabPath);
    [AssetPath.Attribute(typeof(GameObject))] public string SettingsForStartScreenPrefabPath;
    public GameObject SettingsForStartScreenPrefab => LoadPrefab(SettingsForStartScreenPrefabPath);
    [AssetPath.Attribute(typeof(GameObject))] public string SpecialThanksPrefabPath;
    public GameObject SpecialThanksPrefab => LoadPrefab(SpecialThanksPrefabPath);

    public GameObject LoadPrefab(string path)
    {
        return AssetPath.Load<GameObject>(path);
    }
}
