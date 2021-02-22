using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using UnityEngine;

public class StartScreenStartedCommand : EventCommand
{
    public override async void Execute()
    {
        if (AudioManager.Instance.GetPlayingMusic() != MusicId.None)
        {
            await AudioManager.Instance.FadeOutAndStopMusicAsync();
        }

        AudioManager.Instance.Play(MusicId.Menu_1);

        //start systems
        injectionBinder.GetInstance<StartScreenControlSystem>().Start();
    }
}
