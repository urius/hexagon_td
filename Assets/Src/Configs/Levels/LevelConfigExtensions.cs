using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelConfigExtensions
{
    public static BoosterId[] GetAvailableBoosters(this LevelConfig levelConfig)
    {
        var boosterConfigs = BoostersConfigProvider.Instance.BoosterConfigs;

        var result = new List<BoosterId>(boosterConfigs.Length);
        for (var i = 0; i < boosterConfigs.Length; i++)
        {
            if (levelConfig.IsBoosterEnabled(boosterConfigs[i].BoosterId))
            {
                result.Add(boosterConfigs[i].BoosterId);
            }
        }

        return result.ToArray();
    }
}
