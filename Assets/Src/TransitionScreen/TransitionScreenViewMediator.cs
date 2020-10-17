using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class TransitionScreenViewMediator : EventMediator
{
    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher globalDispatcher { get; set; }
    [Inject] public TransitionScreenView View { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        View.ShowAnimationEnded += OnShowAnimationEnded;
        View.HideAnimationEnded += OnHideAnimationEnded;
        globalDispatcher.AddListener(MediatorEvents.UI_TS_REQUEST_HIDE_ANIM, OnRequestHide);
        globalDispatcher.AddListener(MediatorEvents.UI_TS_LOAD_GAME_PROGRESS_UPDATED, OnLoadProgressUpdated);
    }

    public void Start()
    {
        View.SetLoadingText(Loc.Get(LocalizationGroupId.TransitionScreen, "loading"));
    }

    private void OnDestroy()
    {
        View.ShowAnimationEnded -= OnShowAnimationEnded;
        View.HideAnimationEnded -= OnHideAnimationEnded;
        globalDispatcher.RemoveListener(MediatorEvents.UI_TS_REQUEST_HIDE_ANIM, OnRequestHide);
        globalDispatcher.RemoveListener(MediatorEvents.UI_TS_LOAD_GAME_PROGRESS_UPDATED, OnLoadProgressUpdated);
    }

    private void OnShowAnimationEnded()
    {
        globalDispatcher.Dispatch(MediatorEvents.UI_TS_SHOW_ANIM_ENDED);
    }

    private void OnLoadProgressUpdated(IEvent payload)
    {
        View.SetLoadPercent((int)((float)payload.data/0.9f*100));
    }

    private void OnRequestHide(IEvent payload)
    {
        View.HideAnim();
    }

    private void OnHideAnimationEnded()
    {
        globalDispatcher.Dispatch(MediatorEvents.UI_TS_HIDE_ANIM_ENDED);
    }
}
