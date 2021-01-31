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
    public GameObject HowToPlayScreenPrefab => LoadPrefab($"{PrefabsUiFolder}/HowToPlayScreen");

    public GameObject LoadPrefab(string path)
    {
        return Resources.Load<GameObject>(path);
    }
}
