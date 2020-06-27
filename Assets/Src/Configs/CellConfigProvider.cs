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
}
