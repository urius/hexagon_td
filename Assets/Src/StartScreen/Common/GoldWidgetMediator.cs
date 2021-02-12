using strange.extensions.mediation.impl;

public class GoldWidgetMediator : EventMediator
{
    [Inject] public GoldWidgetView WidgetView { get; set; }

    private PlayerGlobalModel _playerModel;
    private bool _isDestroyed = false;

    public void Awake()
    {
        _playerModel = PlayerGlobalModelHolder.Model;
    }

    public override void OnRegister()
    {
        base.OnRegister();

        WidgetView.ButtonClicked += OnButtonClicked;
        _playerModel.GoldAmountUpdated += OnGoldAmountUpdated;
    }

    public async void Start()
    {
        WidgetView.SetAmount(PlayerGlobalModelHolder.Model.Gold);

        await IAPManager.Instance.InitializedTask;
        if (!_isDestroyed)
        {
            WidgetView.ToShowState();
        }
    }

    public override void OnRemove()
    {
        WidgetView.ButtonClicked -= OnButtonClicked;
        _playerModel.GoldAmountUpdated -= OnGoldAmountUpdated;
        _isDestroyed = true;

        base.OnRemove();
    }

    private void OnGoldAmountUpdated()
    {
        WidgetView.SetAmountAnimated(_playerModel.Gold);
    }

    private void OnButtonClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_GOLD_CLICKED);
    }
}
