using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelByViewHolder : IUnitModelByViewProvider, IUnitViewsProvider, ITurretModelByViewProvider
{
    private readonly Dictionary<UnitView, UnitModel> _unitModelByView = new Dictionary<UnitView, UnitModel>();
    private readonly Dictionary<TurretView, TurretModel> _turretModelByView = new Dictionary<TurretView, TurretModel>();

    public List<UnitView> UnitViews { get; } = new List<UnitView>();

    public UnitModel GetModel(UnitView unitView)
    {
        return _unitModelByView[unitView];
    }

    public TurretModel GetModel(TurretView unitView)
    {
        return _turretModelByView[unitView];
    }

    public void Remove(UnitModel unitModel)
    {
        var view = GetViewByModel(unitModel);
        _unitModelByView.Remove(view);
        UnitViews.Remove(view);
    }

    public void Add(UnitView unitView, UnitModel unitModel)
    {
        _unitModelByView[unitView] = unitModel;
        UnitViews.Add(unitView);
    }

    public void Add(TurretView turretView, TurretModel turretModel)
    {
        _turretModelByView[turretView] = turretModel;
    }

    public UnitView GetViewByModel(UnitModel unitModel)
    {
        return _unitModelByView.First(kvp => kvp.Value == unitModel).Key;
    }
}

public interface IUnitModelByViewProvider
{
    UnitModel GetModel(UnitView unitView);
}

public interface IUnitViewsProvider
{
    List<UnitView> UnitViews { get; }

    UnitView GetViewByModel(UnitModel unitModel);
}

public interface ITurretModelByViewProvider
{
    TurretModel GetModel(TurretView unitView);
}
