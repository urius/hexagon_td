using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelByViewHolder : IUnitModelByViewProvider, IUnitViewsProvider
{
    private readonly Dictionary<UnitView, UnitModel> _unitModelByView = new Dictionary<UnitView, UnitModel>();

    public List<UnitView> UnitViews { get; } = new List<UnitView>();

    public UnitModel GetModel(UnitView unitView)
    {
        return _unitModelByView[unitView];
    }

    public bool TryGetModel(UnitView unitView, out UnitModel unitModel)
    {
        return _unitModelByView.TryGetValue(unitView, out unitModel);
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

    public UnitView GetViewByModel(UnitModel unitModel)
    {
        return _unitModelByView.First(kvp => kvp.Value == unitModel).Key;
    }
}

public interface IUnitModelByViewProvider
{
    UnitModel GetModel(UnitView unitView);
    bool TryGetModel(UnitView unitView, out UnitModel unitModel);
}

public interface IUnitViewsProvider
{
    List<UnitView> UnitViews { get; }

    UnitView GetViewByModel(UnitModel unitModel);
}
