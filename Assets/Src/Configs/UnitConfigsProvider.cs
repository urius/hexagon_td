using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/UnitConfigsProvider", fileName = "UnitConfigsProvider")]
public class UnitConfigsProvider : ScriptableObject
{
    public UnitConfig[] Configs;
    public UnitSkinConfig[] Skins;

    public UnitConfig GetConfigByType(UnitTypeMin type)
    {
        return Configs.FirstOrDefault(c => c.UnitType == type);
    }

    public UnitSkinConfig GetSkinConfigBySkin(UnitSkin skin)
    {
        return Skins.FirstOrDefault(c => c.Skin == skin);
    }
}

[Serializable]
public class UnitConfig
{
    public UnitTypeMin UnitType;
    public UnitSkin Skin;
    public int HP = 1;
    public float Speed = 3;
}

[Serializable]
public class UnitSkinConfig
{
    public UnitSkin Skin;
    public GameObject Prefab;
    public GameObject ExplosionPrefab;
}

public enum UnitTypeMin
{
    Armor_1_slow,
    Armor_1_fast,
    Armor_1_superfast,
    Armor_2_slow,
    Armor_2_fast,
    Armor_2_superfast,
    Armor_3_slow,
    Armor_3_fast,
    Armor_3_superfast,
    Armor_4_slow,
    Armor_4_fast,
    Armor_4_superfast,
    Armor_5_superslow,
    Armor_5_slow,
    Armor_5_fast,
    Armor_6_superslow,
    Armor_6_slow,
    Armor_6_fast,
    Armor_7_superslow,
    Armor_7_slow,
    Armor_7_fast,
    Armor_8_superslow,
    Armor_8_slow,
    Armor_8_fast,
    Armor_9_superslow,
    Armor_9_slow,
    Armor_9_fast,
    Armor_10_superslow,
    Armor_10_slow,
    Armor_10_fast,
    Armor_11_superslow,
    Armor_11_slow,
    Armor_11_fast,
    Armor_12_superslow,
    Armor_12_slow,
    Armor_12_fast,
}

public enum UnitSkin
{
    None,
    Green_lite,
    Indigo,
    Gray_with_red_1,
    Dark_green,
    Red_with_blue,
    Green_with_brown,
    Gray_with_red_2,
    Orange_with_green_armor,
    Blue_with_red_heavy,
    Red_with_orange_heavy,
}
