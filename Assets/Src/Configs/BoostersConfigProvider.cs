using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/BoostersConfigProvider", fileName = "BoostersConfigProvider")]
public class BoostersConfigProvider : ScriptableObject
{
    public static BoostersConfigProvider Instance { get; private set; }

    public BoosterConfig[] BoosterConfigs;

    private void OnEnable()
    {
        Instance = this;
    }
}

[Serializable]
public class BoosterConfig
{
    public BoosterId BoosterId;
    public Sprite IconSprite;
    public int Value;
    public int Price;
    public string LocaleTitleKey;
    public string LocaleDescriptionKey;
}

public enum BoosterId
{
    Undefined,
    StartMoneyX2,
    IncreasedReward,
    BaseCapacityX2,
    RepairBaseAfterWave_1,
    RepairBaseAfterWave_10,
    ExtraTurretsPower,
    SlowEnemies,
}
