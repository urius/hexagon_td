using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public abstract class TurretMediatorBase
{
    [Inject(ContextKeys.CONTEXT_DISPATCHER)]
    public IEventDispatcher dispatcher { get; set; }

    [Inject] public IUnitModelByViewProvider UnitModelByViews { get; set; }
    [Inject] public ICellSizeProvider CellSizeProvider { get; set; }
    [Inject] public TurretConfigProvider TurretsConfigProvider { get; set; }
    [Inject] public ICellPositionConverter CellPositionConverter { get; set; }
    [Inject] public UIPrefabsConfig UIPrefabsConfig { get; set; }

    protected Vector3 SelfPosition;
    protected double AttackRadiusSqr;
    protected TurretModel TurretModel;

    private TurretRadiusView _turretRadius;
    private GameObject _turretSelection;
    private GameObject TurretViewGo;

    public virtual void Initialize(TurretModel turretModel)
    {
        TurretModel = turretModel;
        TurretViewGo = CreateView(turretModel);
        Activate();
    }

    protected abstract GameObject CreateView(TurretModel turretModel);

    protected virtual void Activate()
    {
        RefreshAttackRadius();

        TurretModel.Upgraded += OnTurretUpgraded;
        TurretModel.Destroyed += OnTurretDestroyed;
        dispatcher.AddListener(CommandEvents.TURRET_SELECTED, OnTurretSelected);
        dispatcher.AddListener(MediatorEvents.TURRET_DESELECTED, OnTurretDeselected);

        SelfPosition = TurretViewGo.transform.position;
    }

    protected virtual void Deactivate()
    {
        TurretModel.Upgraded -= OnTurretUpgraded;
        TurretModel.Destroyed -= OnTurretDestroyed;
        dispatcher.RemoveListener(CommandEvents.TURRET_SELECTED, OnTurretSelected);
        dispatcher.RemoveListener(MediatorEvents.TURRET_DESELECTED, OnTurretDeselected);
    }

    protected virtual void OnTurretDestroyed()
    {
        Deactivate();

        DestroyTurretSelectedGUIView();
        DestroyView();
    }

    protected virtual void OnTurretUpgraded()
    {
        DestroyView();
        TurretViewGo = CreateView(TurretModel);

        RefreshAttackRadius();

        GameObject.Instantiate(UIPrefabsConfig.UpgradePSPrefab, TurretViewGo.transform.position, Quaternion.identity);
    }

    protected virtual void RefreshAttackRadius()
    {
        AttackRadiusSqr = Math.Pow(TurretModel.AttackRadiusCells * CellSizeProvider.CellSize.x, 2);
    }

    private void OnTurretSelected(IEvent payload)
    {
        var turetModel = (TurretModel)payload.data;

        DestroyTurretSelectedGUIView();
        if (TurretModel == turetModel)
        {
            var turretRadiusGo = GameObject.Instantiate(UIPrefabsConfig.TurretRadiusPrefab);
            _turretRadius = turretRadiusGo.GetComponent<TurretRadiusView>();
            _turretRadius.transform.position = SelfPosition;
            _turretRadius.SetSize(2 * (float)Math.Sqrt(AttackRadiusSqr));

            _turretSelection = GameObject.Instantiate(UIPrefabsConfig.TurretSelectionPrefab);
            _turretSelection.transform.position = SelfPosition;
        }
    }

    private void DestroyTurretSelectedGUIView()
    {
        if (_turretRadius != null)
        {
            GameObject.Destroy(_turretRadius.gameObject);
            _turretRadius = null;
        }

        if (_turretSelection != null)
        {
            GameObject.Destroy(_turretSelection);
            _turretSelection = null;
        }
    }

    private void OnTurretDeselected(IEvent payload)
    {
        DestroyTurretSelectedGUIView();
    }

    private void DestroyView()
    {
        GameObject.Destroy(TurretViewGo);
    }
}
