using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameObject : MonoBehaviour
{
    [SerializeField] private LocalizationProvider _localizationProvider;
    [SerializeField] private CommonUIPrefabsConfig _commonUIPrefabsConfig;
    [SerializeField] private LevelsCollectionProvider _levelsCollectionProvider;
    [SerializeField] private UIPrefabsConfig _uiPrefabsConfig;
    [SerializeField] private BoostersConfigProvider _boostersConfigProvider;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
