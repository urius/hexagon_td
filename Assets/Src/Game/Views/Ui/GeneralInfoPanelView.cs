using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class GeneralInfoPanelView : View
{
    [SerializeField] private Text _text;
    [SerializeField] private Animation _animation;

    private TaskCompletionSource<bool> _playShowTsc;
    private TaskCompletionSource<bool> _playHideTsc;

    protected override void Awake()
    {
        base.Awake();

        _text.text = string.Empty;
    }

    public Task ShowAsync()
    {
        if (_animation.Play("GeneralInfoPanel_show"))
        {
            _playShowTsc = new TaskCompletionSource<bool>();
            return _playShowTsc.Task;
        }
        return Task.CompletedTask;
    }

    public Task HideAsync()
    {
        if (_animation.Play("GeneralInfoPanel_hide"))
        {
            _playHideTsc = new TaskCompletionSource<bool>();
            return _playHideTsc.Task;
        }
        return Task.CompletedTask;
    }

    public async Task SetTextAsync(string text, CancellationToken stopToken = default)
    {
        ClearText();

        foreach (var ch in text)
        {
            _text.text += ch;
            await Task.Delay(35, stopToken).ContinueWith(_ => { });
            if (stopToken.IsCancellationRequested)
            {
                _text.text = text;
                break;
            }
        }
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void EventOnShowAnimationFinished()
    {
        _playShowTsc.SetResult(true);
    }

    public void EventOnHideAnimationFinished()
    {
        _playHideTsc.SetResult(true);
    }

    private void ClearText()
    {
        _text.text = string.Empty;
    }
}
