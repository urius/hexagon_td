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

    private LevelsScrollViewItem[] _itemViews;
    private RectTransform[] _containers;
    private float _itemContainerWidth;

    public override void OnRegister()
    {
        base.OnRegister();

        SelectLevelScrollView.ScrollRightClicked += OnRightClick;
        SelectLevelScrollView.ScrollLeftClicked += OnLeftClick;
    }

    public override void OnRemove()
    {
        base.OnRemove();

        SelectLevelScrollView.ScrollRightClicked -= OnRightClick;
        SelectLevelScrollView.ScrollLeftClicked -= OnLeftClick;
    }

    public void Start()
    {
        _itemContainerWidth = ((RectTransform)UIPrefabsConfig.SelectLevelItemContainerPrefab.transform).rect.width;

        CreateItems(LevelsCollectionProvider.Levels.Length);
    }

    public void CreateItems(int count)
    {
        _itemViews = new LevelsScrollViewItem[count];

        var containersNum = count / ItemsContainerCapacity + 1;

        RectTransform CreateContainer(int index)
        {
            var containerIndex = index / ItemsContainerCapacity;
            return Instantiate(UIPrefabsConfig.SelectLevelItemContainerPrefab, SelectLevelScrollView.ContentTransform).GetComponent<RectTransform>();
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

    private void OnRightClick()
    {
        TweenFactory.RemoveTweenKey(ScrollTweenId, TweenStopBehavior.DoNotModify);

        var newPosition = SelectLevelScrollView.ContentTransform.anchoredPosition.x - _itemContainerWidth - 1;

        var bound = _itemContainerWidth - SelectLevelScrollView.ContentTransform.rect.width;
        if (newPosition < bound)
        {
            newPosition = bound;
        }

        ScrollTo(newPosition);
    }

    private void OnLeftClick()
    {
        TweenFactory.RemoveTweenKey(ScrollTweenId, TweenStopBehavior.DoNotModify);

        var newPosition = SelectLevelScrollView.ContentTransform.anchoredPosition.x + _itemContainerWidth + 1;

        var bound = 0;
        if (newPosition > bound)
        {
            newPosition = bound;
        }

        ScrollTo(newPosition);
    }

    private void ScrollTo(float newPosition)
    {
        SelectLevelScrollView.ContentTransform.gameObject.Tween(ScrollTweenId,
            SelectLevelScrollView.ContentTransform.anchoredPosition,
            new Vector2(newPosition, SelectLevelScrollView.ContentTransform.anchoredPosition.y), 0.5f, TweenScaleFunctions.CubicEaseOut, OnScrollTween);
    }

    private void OnScrollTween(ITween<Vector2> tween)
    {
        SelectLevelScrollView.ContentTransform.anchoredPosition = tween.CurrentValue;
    }
}
