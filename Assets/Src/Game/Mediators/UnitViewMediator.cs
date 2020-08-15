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
            var movingState = _unitModel.CurrentState as MovingState;
            if (movingState.IsTeleporting)
            {
                await TeleportAsync();
            }
            else
            {
                await RotateUnitAsync(GetTargetRotation(false));//in case of changing next cell position just before moving
                await MoveUnitAsync(movingState.SpeedMultiplier);
                if (_unitModel.IsNextCellNear && !_unitModel.IsOnLastCell)
                {
                    await RotateUnitAsync(GetTargetRotation(true));
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
        await RotateUnitAsync(GetTargetRotation(true), 0);
        await Task.Delay(300);

        DispatchHalfStatePassed();
    }

    private Task RotateUnitAsync(Quaternion targetRotation, float duration = 0.2f)
    {
        var tsc = new TaskCompletionSource<bool>();

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

    private Quaternion GetTargetRotation(bool isAfterMove)
    {
        var toPos = isAfterMove ? _unitModel.NextCellPosition : _unitModel.CurrentCellPosition;
        var fromPos = isAfterMove ? _unitModel.CurrentCellPosition : _unitModel.PreviousCellPosition;
        var lookAtVector = cellPositionConverter.CellVec2ToWorld(toPos) - cellPositionConverter.CellVec2ToWorld(fromPos);
        return Quaternion.LookRotation(lookAtVector);
    }

    private Task MoveUnitAsync(float speedMultiplier)
    {
        var duration = 0.5f / speedMultiplier;
        var halfDuration = duration * 0.3f;
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
