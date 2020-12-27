using System;
using strange.extensions.dispatcher.eventdispatcher.api;
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

        dispatcher.AddListener(CommandEvents.UI_REQUEST_ANIMATE_WAVE_REWARD, OnRequestAnimateWaveReward);
    }

    public override void OnRemove()
    {
        dispatcher.RemoveListener(CommandEvents.UI_REQUEST_ANIMATE_WAVE_REWARD, OnRequestAnimateWaveReward);

        LevelModel.MoneyAmountUpdated -= OnMoneyAmountUpdated;
        LevelModel.InsufficientMoneyTriggered -= OnInsufficientMoneyTriggered;

        base.OnRemove();
    }

    private void OnRequestAnimateWaveReward(IEvent payload)
    {
        dispatcher.Dispatch(MediatorEvents.EARN_MONEY_ANIMATION, new MediatorEventsParams.EarnMoneyAnimationParams(View.transform.position, (int)payload.data));
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
