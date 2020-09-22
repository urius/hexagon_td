using System;
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
    [Inject] public IUpdateProvider updateProvider { get; set; }
    [Inject] public UIPrefabsConfig UiPrefabsConfig { get; set; }
    [Inject] public IViewManager ViewManager { get; set; }

    private UnitModel _unitModel;
    private List<object> _tweeners = new List<object>(3);

    private bool IsDestroying => _unitModel.IsDestroying;

    private Action _updateDelegate;
    private int _stateStep = 0;
    private Quaternion _targetRotation;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private int _stepDurationFrames;
    private int _stepPartDurationFrames;
    private int _stepFramesPassed;

    public override void OnRegister()
    {
        _unitModel = modelByView.GetModel(unitView);

        _unitModel.StateUpdated += OnStateUpdated;
        _unitModel.EarnMoneyAnimationTriggered += OnEarnMoneyTriggered;
        updateProvider.UpdateAction += OnUpdate;
    }

    public void OnDestroy()
    {
        StopTweeners();
        _unitModel.StateUpdated -= OnStateUpdated;
        _unitModel.EarnMoneyAnimationTriggered -= OnEarnMoneyTriggered;
        updateProvider.UpdateAction -= OnUpdate;
    }

    private void OnEarnMoneyTriggered(int moneyAmount)
    {
        dispatcher.Dispatch(MediatorEvents.EARN_MONEY_ANIMATION,
            new MediatorEventsParams.EarnMoneyAnimationParams(unitView.transform.position, moneyAmount));
    }

    private void OnUpdate()
    {
        _updateDelegate?.Invoke();
    }

    private void OnStateUpdated()
    {
        _stateStep = 0;
        _updateDelegate = null;

        if (_unitModel.CurrentState.StateName == UnitStateName.Moving)
        {
            var movingState = _unitModel.CurrentState as MovingState;
            if (movingState.IsTeleporting)
            {
                _updateDelegate = ProcessTeleporing;
            }
            else
            {
                _updateDelegate = ProcessMoving;
            }
        }
        else if (_unitModel.IsDestroying)
        {
            if(_unitModel.HP > 0)
            {
                Instantiate(UiPrefabsConfig.ExplosionGoalPrefab, unitView.transform.position, unitView.transform.rotation);
            } else
            {
                ViewManager.Instantiate(_unitModel.ExplosionPrefab, unitView.transform.position, unitView.transform.rotation);
            }
            
            dispatcher.Dispatch(MediatorEvents.UNIT_DESTROY_ANIMATION_FINISHED, unitView);
        }
    }

    private void ProcessTeleporing()
    {
        switch (_stateStep)
        {
            case 0:
                _stepFramesPassed = 0;
                _stateStep++;
                break;
            case 1:
                _stepFramesPassed++;
                if (_stepFramesPassed > 24)
                {
                    _stepFramesPassed = 0;
                    transform.position = cellPositionConverter.CellVec2ToWorld(_unitModel.CurrentCellPosition);
                    transform.rotation = GetTargetRotation(true);
                    _stateStep++;
                }
                break;
            case 2:
                _stepFramesPassed++;
                if (_stepFramesPassed > 18)
                {
                    _stateStep++;
                    DispatchHalfStatePassed();
                    DispatchMoveFinished();
                }
                break;
        }
    }

    private void ProcessMoving()
    {
        switch (_stateStep)
        {
            case 0:
                _targetRotation = GetTargetRotation(false);
                PrepareMotionStep(cellPositionConverter.CellVec2ToWorld(_unitModel.CurrentCellPosition));
                _stateStep++;
                if (ProcessRotationStep())
                {
                    ProcessMotionStep();
                }
                break;
            case 1:
                ProcessRotationStep();
                break;
            case 2:
                ProcessMotionStep();
                if (_stepFramesPassed > _stepPartDurationFrames)
                {
                    _stateStep++;
                    DispatchHalfStatePassed();
                }
                break;
            case 3:
                if (ProcessMotionStep())
                {
                    if (_unitModel.IsNextCellNear && !_unitModel.IsOnLastCell)
                    {
                        _targetRotation = GetTargetRotation(true);
                        DispatchPreRotationStep();
                        if (ProcessRotationStep())
                        {
                            _stateStep = -1;
                            DispatchMoveFinished();
                        }
                    }
                    else
                    {
                        _stateStep = -1;
                        DispatchMoveFinished();
                    }
                }
                break;
            case 4:
                ProcessRotationStep();
                break;
            case 5:
                _stateStep++;
                DispatchMoveFinished();
                break;
        }
    }

    private void PrepareMotionStep(Vector3 targetPosition)
    {
        _startPosition = transform.position;
        _targetPosition = targetPosition;
        _stepDurationFrames = (int) (60 / _unitModel.Speed);
        _stepPartDurationFrames = (int)(_stepDurationFrames * 0.3f);
        _stepFramesPassed = 0;
    }

    private bool ProcessMotionStep()
    {
        var f = (float)_stepFramesPassed / _stepDurationFrames;
        transform.position = Vector3.Lerp(_startPosition, _targetPosition, f);
        if (_stepFramesPassed >= _stepDurationFrames)
        {
            _stateStep++;
            return true;
        }
        _stepFramesPassed++;
        return false;
    }

    private bool ProcessRotationStep()
    {
        var rotationBuf = transform.rotation;
        var newRotation = Quaternion.RotateTowards(rotationBuf, _targetRotation, _unitModel.Speed);
        transform.rotation = newRotation;
        if (Quaternion.Angle(rotationBuf, newRotation) < 1)
        {
            _stateStep++;
            return true;
        }
        return false;
    }

    private void StopTweeners()
    {
        _tweeners.ForEach(s =>
        {
            DOTween.Pause(s);
            DOTween.Kill(s);
        });
        _tweeners.Clear();
    }

    private async Task TeleportAsync()
    {
        await Task.Delay(400);

        if (IsDestroying) return;
        unitView.transform.position = cellPositionConverter.CellVec2ToWorld(_unitModel.CurrentCellPosition);
        await RotateUnitAsync(GetTargetRotation(true), 0);

        if (IsDestroying) return;
        await Task.Delay(300);

        if (IsDestroying) return;
        DispatchHalfStatePassed();
    }

    private Task RotateUnitAsync(Quaternion targetRotation, float duration = 0.2f)
    {
        var tsc = new TaskCompletionSource<bool>();

        if (Quaternion.Angle(targetRotation, unitView.transform.rotation) > 1)
        {
            var tweener = unitView.transform.DORotate(targetRotation.eulerAngles, duration);
            tweener.OnComplete(() => tsc.TrySetResult(true));
            _tweeners.Add(tweener.id);
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
        tweener.OnComplete(() => tsc.TrySetResult(true));
        _tweeners.Add(tweener.id);

        var delayedCall = DOVirtual.DelayedCall(halfDuration, DispatchHalfStatePassed);
        _tweeners.Add(delayedCall.id);

        return tsc.Task;
    }

    private void DispatchMoveFinished()
    {
        dispatcher.Dispatch(MediatorEvents.UNIT_MOVE_TO_NEXT_CELL_FINISHED, _unitModel);
    }

    private void DispatchHalfStatePassed()
    {
        dispatcher.Dispatch(MediatorEvents.UNIT_HALF_STATE_PASSED, _unitModel);
    }

    private void DispatchPreRotationStep()
    {
        dispatcher.Dispatch(MediatorEvents.UNIT_BEFORE_ROTATION, _unitModel);
    }
}
