using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class GeneralInfoPanelView : View
{
    [SerializeField] private Text _text;
    [SerializeField] private Animation _animation;

    private UniTaskCompletionSource<bool> _playShowTsc;
    private UniTaskCompletionSource<bool> _playHideTsc;

    protected override void Awake()
    {
        base.Awake();

        _text.text = string.Empty;
    }

    public UniTask ShowAsync()
    {
        if (_animation.Play("GeneralInfoPanel_show"))
        {
            _playShowTsc = new UniTaskCompletionSource<bool>();
            return _playShowTsc.Task;
        }
        return UniTask.CompletedTask;
    }

    public void SetTextColor(Color color)
    {
        _text.color = color;
    }

    public UniTask HideAsync()
    {
        if (_animation.Play("GeneralInfoPanel_hide"))
        {
            _playHideTsc = new UniTaskCompletionSource<bool>();
            return _playHideTsc.Task;
        }
        return UniTask.CompletedTask;
    }

    public async UniTask SetTextAsync(string text, CancellationToken stopToken = default)
    {
        ClearText();

        foreach (var ch in text)
        {
            _text.text += ch;
            await UniTask.Delay(35, cancellationToken: stopToken);
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
        _playShowTsc.TrySetResult(true);
    }

    public void EventOnHideAnimationFinished()
    {
        _playHideTsc.TrySetResult(true);
    }

    private void ClearText()
    {
        _text.text = string.Empty;
    }
}
