using System;

[Serializable]
public class WaveSetting
{
    public WavePartSetting[] WaveParts;
    public int[] DisabledBaseIndices;

    public WaveSetting()
    {
    }
}

[Serializable]
public class WavePartSetting
{
    public UnitTypeMin UnitType;
    public UnitSkin OverrideSkin;
    public int Amount;
}
