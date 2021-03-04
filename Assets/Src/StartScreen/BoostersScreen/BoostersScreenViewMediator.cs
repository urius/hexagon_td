using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using strange.extensions.mediation.impl;
using UnityEngine;

public class BoostersScreenViewMediator : EventMediator
{
    [Inject] public BoostersScreenView View { get; set; }

    private readonly IDictionary<BoosterId, BoosterItemView> _itemsViews = new Dictionary<BoosterId, BoosterItemView>();

    private Action _disposeAction = delegate { };

    public override void OnRegister()
    {
        base.OnRegister();

        var sessionModel = PlayerSessionModel.Instance;

        View.PlayClicked += OnPlayClicked;
        sessionModel.SelectedLevelData.BoosterAdded += OnBoosterAdded;
        sessionModel.SelectedLevelData.BoosterRemoved += OnBoosterRemoved;
    }

    public void Start()
    {
        SetupTexts();
        CreateBoosterItems();
    }

    public override void OnRemove()
    {
        var sessionModel = PlayerSessionModel.Instance;

        View.PlayClicked -= OnPlayClicked;
        sessionModel.SelectedLevelData.BoosterAdded -= OnBoosterAdded;
        sessionModel.SelectedLevelData.BoosterRemoved -= OnBoosterRemoved;

        _disposeAction();

        base.OnRemove();
    }

    private void SetupTexts()
    {
        var loc = LocalizationProvider.Instance;
        var levelNumber = PlayerSessionModel.Instance.SelectedLevelIndex + 1;
        var levelNameLocaleKey = PlayerSessionModel.Instance.SelectedLevelData.LevelConfig.NameKey;
        var levelTitle = string.Format(loc.Get(LocalizationGroupId.BoostersScreen, "level_title"), levelNumber)
            + $"\n\"{loc.Get(LocalizationGroupId.LevelNames, levelNameLocaleKey)}\"";

        var boostersTitle = loc.Get(LocalizationGroupId.BoostersScreen, "boosters_title");
        View.SetupTexts(levelTitle, boostersTitle);
    }

    private void CreateBoosterItems()
    {
        var selectedLevelData = PlayerSessionModel.Instance.SelectedLevelData;
        var availableBoosters = selectedLevelData.LevelConfig.GetAvailableBoosters();
        System.Random rnd = new System.Random();
        availableBoosters = availableBoosters.OrderBy(b => rnd.Next()).ToArray();

        var loc = LocalizationProvider.Instance;
        foreach (var boosterId in availableBoosters)
        {
            var boosterConfig = BoostersConfigProvider.GetBoosterConfigById(boosterId);
            var boosterGo = Instantiate(UIPrefabsConfig.Instance.BoosterItemPrefab, View.ContentContainerTransform);
            var boosterItemView = boosterGo.GetComponent<BoosterItemView>();
            _itemsViews.Add(boosterId, boosterItemView);

            var title = loc.Get(LocalizationGroupId.BoostersScreen, boosterConfig.LocaleTitleKey);
            var description = string.Format(loc.Get(LocalizationGroupId.BoostersScreen, boosterConfig.LocaleDescriptionKey), boosterConfig.Value);
            boosterItemView.Setup(boosterConfig.IconSprite,
                title,
                description,
                boosterConfig.Price.ToSpaceSeparatedAmount());
            boosterItemView.SetCheckedState(selectedLevelData.IsBoosterSelected(boosterId));

            var boosterIdScoped = boosterId;
            void OnClickHandler()
            {
                OnBoosterClick(boosterIdScoped);
            }
            boosterItemView.CLicked += OnClickHandler;
            _disposeAction += () => boosterItemView.CLicked -= OnClickHandler;
        }
    }

    private void OnBoosterRemoved(BoosterId boosterId)
    {
        if (_itemsViews.TryGetValue(boosterId, out var itemView))
        {
            itemView.SetCheckedState(false);
        }
    }

    private void OnBoosterAdded(BoosterId boosterId)
    {
        if (_itemsViews.TryGetValue(boosterId, out var itemView))
        {
            itemView.SetCheckedState(true);
        }
    }

    private void OnBoosterClick(BoosterId boosterId)
    {
        dispatcher.Dispatch(MediatorEvents.UI_BOOSTERS_SCREEN_BOOSTER_CLICKED, boosterId);
    }

    private void OnPlayClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_BOOSTERS_SCREEN_PLAY_LEVEL_CLICKED);
    }
}
