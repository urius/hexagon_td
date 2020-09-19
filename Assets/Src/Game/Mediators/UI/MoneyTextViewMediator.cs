using System;
using strange.extensions.mediation.impl;

public class MoneyTextViewMediator : EventMediator
{
    [Inject] public MoneyTextView View { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        View.SetAmount(LevelModel.Money, false);

        LevelModel.MoneyAmountUpdated += OnMoneyAmountUpdated;
        LevelModel.InsufficientMoneyTriggered += OnInsufficientMoneyTriggered;
    }

    public override void OnRemove()
    {
        base.OnRemove();

        LevelModel.MoneyAmountUpdated -= OnMoneyAmountUpdated;
        LevelModel.InsufficientMoneyTriggered -= OnInsufficientMoneyTriggered;
    }

    private void OnMoneyAmountUpdated()
    {
        View.SetAmount(LevelModel.Money, true);
    }

    private void OnInsufficientMoneyTriggered()
    {
        View.Blink();
    }
}
