using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/WaveConfig", fileName = "Wave_N_description")]
public class WaveConfig : ScriptableObject
{
    public UnitTypeMin[] Units;
    public WillSpawnFromBaseConfig[] WillSpawnFromBaseConfigs;

    private void OnEnable()
    {
        WillSpawnFromBaseConfigs = new WillSpawnFromBaseConfig[0];
    }
}

[Serializable]
public class WillSpawnFromBaseConfig
{
    public WillSpawnFromBaseConfig(Vector2Int baseCellPosition)
    {
        BaseCellPosition = baseCellPosition;
    }

    public Vector2Int BaseCellPosition;
    public bool WillSpawnFrom = true;
}
