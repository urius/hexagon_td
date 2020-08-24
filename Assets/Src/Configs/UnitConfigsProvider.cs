using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/UnitConfigsProvider", fileName = "UnitConfigsProvider")]
public class UnitConfigsProvider : ScriptableObject
{
    public UnitConfig[] Configs;

    public UnitConfig GetConfigByType(UnitTypeMin type)
    {
        return Configs.FirstOrDefault(c => c.UnitType == type);
    }
}

[Serializable]
public class UnitConfig
{
    public UnitTypeMin UnitType;
    public GameObject Prefab;
    public int HP = 1;
    public int Speed = 5;
}

public enum UnitTypeMin
{
    Tank,
}
