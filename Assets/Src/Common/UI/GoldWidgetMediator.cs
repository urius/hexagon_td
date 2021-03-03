using System;
using System.Threading.Tasks;
using DigitalRuby.Tween;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class GoldWidgetMediator : EventMediator
{
    [Inject] public GoldWidgetView WidgetView { get; set; }

    private PlayerGlobalModel _playerModel;
    private bool _isDestroyed = false;

    public override void OnRegister()
    {
        base.OnRegister();

        _playerModel = PlayerSessionModel.Model;

        WidgetView.ButtonClicked += OnButtonClicked;
        _playerModel.GoldAnimationRequested += OnGoldAnimationRequested;
        _playerModel.GoldAmountUpdated += OnGoldAmountUpdated;
    }

    public async void Start()
    {
        WidgetView.SetAmount(PlayerSessionModel.Model.Gold);

        await IAPManager.Instance.InitializedTask;
        if (!_isDestroyed)
        {
            WidgetView.ToShowState();
        }
    }

    public override void OnRemove()
    {
        WidgetView.ButtonClicked -= OnButtonClicked;
        _playerModel.GoldAnimationRequested -= OnGoldAnimationRequested;
        _playerModel.GoldAmountUpdated -= OnGoldAmountUpdated;

        _isDestroyed = true;

        base.OnRemove();
    }

    private void OnGoldAmountUpdated(int amount)
    {
        WidgetView.SetAmountAnimated(_playerModel.Gold);
    }

    private async void OnGoldAnimationRequested(int amount)
    {
        if (amount > 0)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, new Vector2(Screen.width, Screen.height), Camera.main,
                out var screenLocalDim);

            var particlesNum = 10;
            if (amount > 1000) particlesNum = 20;
            if (amount > 10000) particlesNum = 50;
            for (var i = 0; i < particlesNum; i++)
            {
                AnimateGoldItem(UnityEngine.Random.insideUnitCircle * screenLocalDim.x, UnityEngine.Random.Range(0.5f, 1f));
            }

            await Task.Delay(500);

            WidgetView.SetAmountAnimated(_playerModel.Gold);
        }
    }

    private void AnimateGoldItem(Vector2 startPos, float duration)
    {
        var widgetRect = WidgetView.RectTransform;
        var targetRect = WidgetView.FlyTargerRectTransform;

        var DNAIconGO = Instantiate(CommonUIPrefabsConfig.Instance.DNAIconPrefab, transform.parent);
        var DNAIconRect = DNAIconGO.GetComponent<RectTransform>();
        var DNAIconImg = DNAIconGO.GetComponent<Image>();

        DNAIconRect.localPosition = startPos;

        var targetLocalPos = widgetRect.parent.InverseTransformPoint(targetRect.parent.TransformPoint(targetRect.localPosition));
        TweenFactory.Tween(DNAIconImg, DNAIconImg.color.a, 0.4f, duration, TweenScaleFunctions.CubicEaseIn,
            t => DNAIconImg.color = new Color(DNAIconImg.color.r, DNAIconImg.color.g, DNAIconImg.color.b, t.CurrentValue));

        TweenFactory.Tween(DNAIconGO, DNAIconRect.localPosition, targetLocalPos, duration, TweenScaleFunctions.CubicEaseIn,
            t => DNAIconRect.localPosition = t.CurrentValue, t => Destroy(DNAIconGO));
    }

    private void OnButtonClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_GOLD_CLICKED);
    }
}
