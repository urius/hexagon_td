using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfigProvider", menuName = "Configs/LevelConfigProvider")]
public class LevelConfigProvider : ScriptableObject
{
    [SerializeField]
    private LevelConfig _levelConfig;
    public LevelConfig LevelConfig => _levelConfig;
}
