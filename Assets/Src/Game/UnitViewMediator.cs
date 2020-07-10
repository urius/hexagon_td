using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using strange.extensions.mediation.impl;
using UnityEngine;

public class UnitViewMediator : EventMediator
{
    [Inject] public ICellPositionConverter cellPositionConverter { get; set; }
    [Inject] public UnitView unitView { get; set; }
    [Inject] public UnitModelByView modelByView { get; set; }

    private UnitModel _unitModel;

    public override void OnRegister()
    {
        _unitModel = modelByView.ModelByView[unitView];

        _unitModel.StateUpdated += OnStateUpdated;
    }

    public void OnDestroy()
    {
        _unitModel.StateUpdated -= OnStateUpdated;
    }

    private async void OnStateUpdated()
    {
        if (_unitModel.State == UnitStateName.Moving)
        {
            await RotateUnitAsync();
            await MoveUnitAsync();

            dispatcher.Dispatch(MediatorEvents.UNIT_MOVE_TO_NEXT_CELL_FINISHED, _unitModel);
        }
        else if (_unitModel.State == UnitStateName.Destroing)
        {
            Destroy(gameObject);
        }
    }

    private Task RotateUnitAsync()
    {
        var tsc = new TaskCompletionSource<bool>();

        var lookAtVector = cellPositionConverter.CellVec2ToWorld(_unitModel.CurrentCellPosition) - cellPositionConverter.CellVec2ToWorld(_unitModel.PreviousCellPosition);
        var rotation = Quaternion.LookRotation(lookAtVector).eulerAngles;
        var tweener = unitView.transform.DORotate(rotation, 0.2f);
        tweener.OnComplete(() => tsc.TrySetResult(true));

        return tsc.Task;
    }

    private Task MoveUnitAsync()
    {
        var tsc = new TaskCompletionSource<bool>();

        var targetPosition = cellPositionConverter.CellVec2ToWorld(_unitModel.CurrentCellPosition);
        var tweener = unitView.transform.DOMove(targetPosition, 0.5f);
        tweener.OnComplete(() => tsc.TrySetResult(true));

        return tsc.Task;
    }
}
