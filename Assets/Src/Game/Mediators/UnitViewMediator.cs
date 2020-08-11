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
    [Inject] public IUnitModelByViewProvider modelByView { get; set; }

    private UnitModel _unitModel;

    public override void OnRegister()
    {
        _unitModel = modelByView.GetModel(unitView);

        _unitModel.StateUpdated += OnStateUpdated;
    }

    public void OnDestroy()
    {
        _unitModel.StateUpdated -= OnStateUpdated;
    }

    private async void OnStateUpdated()
    {
        if (_unitModel.CurrentState.StateName == UnitStateName.Moving)
        {
            if ((_unitModel.CurrentState as MovingState).IsTeleporting)
            {
                await TeleportAsync();
            }
            else
            {
                await MoveUnitAsync();
                if (_unitModel.IsNextCellNear && !_unitModel.IsOnLastCell)
                {
                    await RotateUnitAsync();
                }
            }

            dispatcher.Dispatch(MediatorEvents.UNIT_MOVE_TO_NEXT_CELL_FINISHED, _unitModel);
        }
        else if (_unitModel.CurrentState.StateName == UnitStateName.Destroing)
        {
            //destroy animation
            dispatcher.Dispatch(MediatorEvents.UNIT_DESTROY_ANIMATION_FINISHED, unitView);
        }
    }

    private async Task TeleportAsync()
    {
        await Task.Delay(400);

        unitView.transform.position = cellPositionConverter.CellVec2ToWorld(_unitModel.CurrentCellPosition);
        await RotateUnitAsync(0);
        await Task.Delay(300);

        DispatchHalfStatePassed();
    }

    private Task RotateUnitAsync(float duration = 0.2f)
    {
        var tsc = new TaskCompletionSource<bool>();

        var lookAtVector = cellPositionConverter.CellVec2ToWorld(_unitModel.NextCellPosition) - cellPositionConverter.CellVec2ToWorld(_unitModel.CurrentCellPosition);
        var targetRotation = Quaternion.LookRotation(lookAtVector);
        if (Quaternion.Angle(targetRotation, unitView.transform.rotation) > 1)
        {
            var tweener = unitView.transform.DORotate(targetRotation.eulerAngles, duration);
            tweener.OnComplete(() => tsc.TrySetResult(true));
            return tsc.Task;
        }
        else
        {
            unitView.transform.rotation = targetRotation;
            return Task.CompletedTask;
        }
    }

    private Task MoveUnitAsync()
    {
        const float duration = 0.5f;
        const float halfDuration = duration * 0.3f;
        var tsc = new TaskCompletionSource<bool>();

        var targetPosition = cellPositionConverter.CellVec2ToWorld(_unitModel.CurrentCellPosition);
        var tweener = unitView.transform.DOMove(targetPosition, duration);
        DOVirtual.DelayedCall(halfDuration, DispatchHalfStatePassed);
        tweener.OnComplete(() => tsc.TrySetResult(true));

        return tsc.Task;
    }

    private void DispatchHalfStatePassed()
    {
        dispatcher.Dispatch(MediatorEvents.UNIT_HALF_STATE_PASSED, _unitModel);
    }
}
