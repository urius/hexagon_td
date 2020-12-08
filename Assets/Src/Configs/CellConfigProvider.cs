using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/CellConfigProvider", fileName = "CellConfigProvider")]
public class CellConfigProvider : ScriptableObject
{
    [SerializeField]
    private CellConfig[] _cellConfigs;

    public CellConfig GetConfig(CellType cellType, CellSubType cellSubType = CellSubType.Default)
    {
        return _cellConfigs.FirstOrDefault(
            c => c.CellConfigMin.CellType == cellType
            && c.CellConfigMin.CellSubType == cellSubType);
    }
}

[Serializable]
public class CellConfigMin
{
    public CellType CellType;
    public CellSubType CellSubType;
}

[Serializable]
public class CellConfig
{
    public GameObject Prefab;
    public CellConfigMin CellConfigMin;
}

[Serializable]
public class CellDataMin
{
    public CellDataMin(Vector2Int cellPosition, CellConfigMin cellConfigMin)
    {
        CellPosition = cellPosition;
        CellConfigMin = cellConfigMin;
    }

    public Vector2Int CellPosition;
    public CellConfigMin CellConfigMin;
}

public enum CellType
{
    Ground,
    Wall,
    EnemyBase,
    Teleport,
    GoalBase,
    Modifier,
}

public static class CellTypeExtensions
{
    public static Enum ConvertToSpecifiedEnum(this CellSubType subType, CellType type)
    {
        switch (type)
        {
            case CellType.Ground:
                return (GroundType)subType;
            case CellType.Wall:
                return (WallType)subType;
            case CellType.EnemyBase:
                return (EnemyBaseType)subType;
            case CellType.Teleport:
                return (TeleportType)subType;
            case CellType.GoalBase:
                return (GoalBaseType)subType;
            case CellType.Modifier:
                return (ModifierType)subType;
            default:
                return subType;
        }
    }
}

public static class CellInfoHelper
{
    public static bool IsRotatableCell(CellType cellType, CellSubType cellSubType)
    {
        return (cellType == CellType.Modifier && !IsDirectionModifier(cellType, cellSubType)) || cellType == CellType.GoalBase;
    }

    private static bool IsDirectionModifier(CellType cellType, CellSubType cellSubType)
    {
        var cellSubTypeInt = (int)cellSubType;
        return cellType == CellType.Modifier
            && (cellSubTypeInt == (int)ModifierType.Direction_30
                || cellSubTypeInt == (int)ModifierType.Direction_90
                || cellSubTypeInt == (int)ModifierType.Direction_150
                || cellSubTypeInt == (int)ModifierType.Direction_210
                || cellSubTypeInt == (int)ModifierType.Direction_270
                || cellSubTypeInt == (int)ModifierType.Direction_330);
    }
}

public enum CellSubType
{
    Default,
    Type_1,
    Type_2,
    Type_3,
    Type_4,
    Type_5,
    Type_6,
    Type_7,
    Type_8,
    Type_9,
    Type_10,
    Type_11,
    Type_12,
    Type_13,
    Type_14,
    Type_15,
}

public enum GroundType
{
    Default,
    Red,
    LinesInner,
}

public enum WallType
{
    Default,
    Red,
    Green,
    Turquoise,
}

public enum EnemyBaseType
{
    Default,
}

public enum TeleportType
{
    Blue,
    Green,
    Red,
    Yellow,
    Purple,
    Orange,
}

public enum GoalBaseType
{
    Default,
}

public enum ModifierType
{
    SpeedUp,
    SpeedDown,
    ExtraMoney,
    ExtraBigMoney,
    Mine,
    Repair,
    NoBuild,
    Direction_30,
    Direction_90,
    Direction_150,
    Direction_210,
    Direction_270,
    Direction_330,
}
