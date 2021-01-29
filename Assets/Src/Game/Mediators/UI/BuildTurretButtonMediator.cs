using System;
using strange.extensions.mediation.impl;

public class BuildTurretButtonMediator : EventMediator
{
    [Inject] public BuildTurretButtonView View { get; set; }
    [Inject] public TurretConfigProvider TurretConfigProvider { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }

    private int _cost;

    public override void OnRegister()
    {
        base.OnRegister();

        View.ButtonPointerDown += OnButtonPointerDown;
        View.ButtonPointerUp += OnButtonPointerUp;
        LevelModel.MoneyAmountUpdated += OnMoneyAmountUpdated;

        _cost = TurretConfigProvider.GetConfig(TurretTypeByTypeId(View.TurretTypeId), 0).Price;
        View.SetCost(_cost);

        RefreshButtonAvailability();
    }

    public override void OnRemove()
    {
        base.OnRemove();

        View.ButtonPointerDown -= OnButtonPointerDown;
        View.ButtonPointerUp -= OnButtonPointerUp;
        LevelModel.MoneyAmountUpdated -= OnMoneyAmountUpdated;
    }

    private void OnButtonPointerDown()
    {
        AudioManager.Instance.Play(SoundId.BuildTurretCanceled);
        dispatcher.Dispatch(MediatorEvents.UI_BUILD_TURRET_MOUSE_DOWN, View.TurretTypeId);
    }

    private void OnButtonPointerUp()
    {
        dispatcher.Dispatch(MediatorEvents.UI_BUILD_TURRET_MOUSE_UP, View.IsUnderMouse);
    }

    private void OnMoneyAmountUpdated()
    {
        RefreshButtonAvailability();
    }

    private void RefreshButtonAvailability()
    {
        View.SetEnabled(LevelModel.Money >= _cost);
    }

    private TurretType TurretTypeByTypeId(int typeId)
    {
        switch (typeId)
        {
            case 0:
                return TurretType.Gun;
            case 1:
                return TurretType.Laser;
            case 2:
                return TurretType.Rocket;
            case 3:
                return TurretType.SlowField;
        }

        throw new System.Exception("Unsuppported turret type id: " + typeId);
    }
}
