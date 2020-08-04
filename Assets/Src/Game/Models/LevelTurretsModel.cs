using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTurretsModel
{
    public readonly List<TurretModel> TurretsList = new List<TurretModel>();
    private readonly Dictionary<Vector2Int, TurretModel> _turrets = new Dictionary<Vector2Int, TurretModel>();

    public void AddTurret(TurretModel turretModel)
    {
        _turrets[turretModel.Position] = turretModel;
        TurretsList.Add(turretModel);
    }

    public void Update()
    {
        foreach (var turret in TurretsList)
        {
            turret.Update();
        }
    }

    public bool HaveTurret(Vector2Int cell)
    {
        return _turrets.ContainsKey(cell);
    }
}
