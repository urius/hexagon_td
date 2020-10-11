using System.Linq;
using DigitalRuby.Tween;
using strange.extensions.mediation.impl;
using UnityEngine;

public class LevelsScrollViewMediator : EventMediator
{
    private const int ItemsContainerCapacity = 12;
    private const string ScrollTweenId = "scrollTween";

    [Inject] public LevelsScrollView SelectLevelScrollView { get; set; }
    [Inject] public LevelsCollectionProvider LevelsCollectionProvider { get; set; }
    [Inject] public UIPrefabsConfig UIPrefabsConfig { get; set; }
    [Inject] public PlayerGlobalModelHolder PlayerGlobalModelHolder { get; set; }

    private LevelsScrollViewItem[] _itemViews;
    private RectTransform[] _containers;
    private RectTransform _contentTransform;
    private float _itemContainerWidth;
    private GameObject _selection;

    public override void OnRegister()
    {
        base.OnRegister();

        SelectLevelScrollView.ScrollRightClicked += OnRightClick;
        SelectLevelScrollView.ScrollLeftClicked += OnLeftClick;
        SelectLevelScrollView.DragEnded += OnDragEnded;
    }

    public override void OnRemove()
    {
        base.OnRemove();

        SelectLevelScrollView.ScrollRightClicked -= OnRightClick;
        SelectLevelScrollView.ScrollLeftClicked -= OnLeftClick;
        SelectLevelScrollView.DragEnded -= OnDragEnded;
    }

    public async void Start()
    {
        await PlayerGlobalModelHolder.ModelInnitializedTask;

        _contentTransform = SelectLevelScrollView.ContentTransform;
        _itemContainerWidth = ((RectTransform)UIPrefabsConfig.SelectLevelItemContainerPrefab.transform).rect.width;

        CreateItems(LevelsCollectionProvider.Levels.Length);
        SetupItems(PlayerGlobalModelHolder.PlayerGlobalModel);
        SubscribeOnItemsClick();

        SelectLevelByIndex(GetFirstUncompletedLevelIndex());
    }

    private int GetFirstUncompletedLevelIndex()
    {
        var result = 0;
        for (var i = 0; i < PlayerGlobalModelHolder.PlayerGlobalModel.LevelsProgress.Length; i++)
        {
            var levelProgress = PlayerGlobalModelHolder.PlayerGlobalModel.LevelsProgress[i];
            if (levelProgress.isUnlocked)
            {
                result = i;
                if (!levelProgress.isPassed)
                {
                    break;
                }
            }
        }

        return result;
    }

    private void CreateItems(int count)
    {
        _itemViews = new LevelsScrollViewItem[count];

        var containersNum = count / ItemsContainerCapacity + 1;

        RectTransform CreateContainer(int index)
        {
            var containerIndex = index / ItemsContainerCapacity;
            return Instantiate(UIPrefabsConfig.SelectLevelItemContainerPrefab, _contentTransform).GetComponent<RectTransform>();
        }

        _containers = Enumerable.Range(0, containersNum).Select(CreateContainer).ToArray();

        LevelsScrollViewItem CreateItem(int index)
        {
            var containerIndex = index / ItemsContainerCapacity;
            var go = Instantiate(UIPrefabsConfig.SelectLevelItemPrefab, _containers[containerIndex]);
            return go.GetComponent<LevelsScrollViewItem>();
        }

        _itemViews = Enumerable.Range(0, count).Select(CreateItem).ToArray();
    }

    private void SetupItems(PlayerGlobalModel playerGlobalModel)
    {
        for (var i = 0; i < _itemViews.Length; i++)
        {
            var itemView = _itemViews[i];
            var levelProgress = playerGlobalModel.LevelsProgress[i];
            itemView.SetLevelNum(i + 1);
            itemView.SetLocked(!levelProgress.isUnlocked);
            itemView.SetPassedMode(levelProgress.isPassed);
            if (levelProgress.isPassed)
            {
                itemView.SetupStars(levelProgress.StarsAmount);
            }
        }
    }

    private void SubscribeOnItemsClick()
    {
        for (var i = 0; i < _itemViews.Length; i++)
        {
            var bufferIndex = i;
            _itemViews[i].OnClick += () => OnLevelClick(bufferIndex);
        }
    }

    private void OnLevelClick(int levelIndex)
    {
        SelectLevelByIndex(levelIndex);
    }

    private void SelectLevelByIndex(int levelIndex)
    {
        if (_selection != null)
        {
            Destroy(_selection);
            _selection = null;
        }

        _selection = Instantiate(UIPrefabsConfig.SelectLevelItemSelectionPrefab, _itemViews[levelIndex].transform);
        var isLocked = !PlayerGlobalModelHolder.PlayerGlobalModel.GetProgressByLevel(levelIndex).isUnlocked;
        _selection.GetComponent<LevelsScrollItemSelectionView>().SetLockedState(isLocked);

        dispatcher.Dispatch(MediatorEvents.UI_SS_SELECT_LEVEL_CLICKED, levelIndex);
    }

    private void OnRightClick()
    {
        var newPosition = _contentTransform.anchoredPosition.x - _itemContainerWidth * 1.5f;
        newPosition = CorrectPosition(newPosition);

        ScrollTo(newPosition);
    }

    private void OnLeftClick()
    {
        var newPosition = _contentTransform.anchoredPosition.x + _itemContainerWidth * 0.5f;
        newPosition = CorrectPosition(newPosition);

        ScrollTo(newPosition);
    }

    private void OnDragEnded()
    {
        var correctedPosition = CorrectPosition(_contentTransform.anchoredPosition.x - _itemContainerWidth * 0.5f);

        ScrollTo(correctedPosition);
    }

    private float CorrectPosition(float newPosition)
    {
        var boundRight = _itemContainerWidth - _contentTransform.rect.width;
        if (newPosition < boundRight)
        {
            return boundRight;
        }

        var boundLeft = 0;
        if (newPosition > boundLeft)
        {
            return boundLeft;
        }

        return ((int)(newPosition / _itemContainerWidth)) * _itemContainerWidth;
    }

    private void ScrollTo(float newPosition)
    {
        TweenFactory.RemoveTweenKey(ScrollTweenId, TweenStopBehavior.DoNotModify);
        SelectLevelScrollView.StopMovement();

        _contentTransform.gameObject.Tween(ScrollTweenId,
            _contentTransform.anchoredPosition,
            new Vector2(newPosition, _contentTransform.anchoredPosition.y), 0.5f, TweenScaleFunctions.CubicEaseOut, OnScrollTween);
    }

    private void OnScrollTween(ITween<Vector2> tween)
    {
        _contentTransform.anchoredPosition = tween.CurrentValue;
    }
}
