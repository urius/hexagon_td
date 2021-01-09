﻿using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using UnityEngine;

public class StartScreenStartedCommand : EventCommand
{
    [Inject] public PlayerGlobalModelHolder PlayerGlobalModelHolder { get; set; }
    [Inject] public DeafultPlayerGlobalModelProvider DeafultPlayerGlobalModelProvider { get; set; }
    [Inject] public LevelsCollectionProvider LevelsCollectionProvider { get; set; }

    public override async void Execute()
    {
        if (AudioManager.Instance.GetPlayingMusic() != MusicId.None)
        {
            await AudioManager.Instance.FadeOutAndStopMusicAsync();
        }

        AudioManager.Instance.Play(MusicId.Menu_1);
        //first time screen opening in game session

        //TODO: move to bootstrapper
        //TODO: load or initialize player model
        /*var playerModel = new PlayerGlobalModel(
            DeafultPlayerGlobalModelProvider.PlayerGlobalModel,
            LevelsCollectionProvider.Levels.Length);
        */
        //PlayerGlobalModelHolder.SetModel(playerModel);
    }
}
