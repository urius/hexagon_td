using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTurretsModel
{
    public event Action<TurretModel> TurretAdded = delegate { };
    public event Action<TurretModel> TurretRemoved = delegate { };

    public readonly List<TurretModel> TurretsList = new List<TurretModel>();

    private readonly Dictionary<Vector2Int, TurretModel> _turrets = new Dictionary<Vector2Int, TurretModel>();

    public void AddTurret(TurretModel turretModel)
    {
        _turrets[turretModel.Position] = turretModel;
        TurretsList.Add(turretModel);

        TurretAdded(turretModel);
    }

    public void RemoveTurret(TurretModel turretModel)
    {
        _turrets.Remove(turretModel.Position);
        TurretsList.Remove(turretModel);

        TurretRemoved(turretModel);
    }

    public bool TryGetTurret(Vector2Int cell, out TurretModel turretModel)
    {
        return _turrets.TryGetValue(cell, out turretModel);
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
