using UnityEngine;

public class SlowFieldTurretMediator : TurretMediatorBase
{
    private TurretView _turretView;

    protected override GameObject CreateView(TurretModel turretModel)
    {
        var turretPrefab = TurretsConfigProvider.GetConfig(turretModel.TurretType, turretModel.TurretConfig.TurretLevelIndex).Prefab;
        var turretViewGo = GameObject.Instantiate(turretPrefab, CellPositionConverter.CellVec2ToWorld(turretModel.Position), Quaternion.identity);
        _turretView = turretViewGo.GetComponent<TurretView>();
        return turretViewGo;
    }
}
