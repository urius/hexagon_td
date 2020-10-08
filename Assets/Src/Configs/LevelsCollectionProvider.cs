using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsCollection", menuName = "Configs/LevelsCollectionProvider")]
public class LevelsCollectionProvider : ScriptableObject
{
    [SerializeField]
    private LevelConfig[] _levels;
    public LevelConfig[] Levels => _levels;
}
