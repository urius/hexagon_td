using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/CommonUIPrefabsConfig", fileName = "CommonUIPrefabsConfig")]
public class CommonUIPrefabsConfig : ScriptableObject
{
    public static CommonUIPrefabsConfig Instance { get; private set; }

    public GameObject GoldStoreWindowPrefab;
    public GameObject DNAIconPrefab;

    private void OnEnable()
    {
        Instance = this;
    }
}
