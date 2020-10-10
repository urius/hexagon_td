using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using UnityEngine;

public class StartScreenStartedCommand : EventCommand
{
    [Inject] public PlayerGlobalModelHolder PlayerGlobalModelHolder { get; set; }
    [Inject] public DeafultPlayerGlobalModelProvider DeafultPlayerGlobalModelProvider { get; set; }
    [Inject] public LevelsCollectionProvider LevelsCollectionProvider { get; set; }

    public override void Execute()
    {
        if (PlayerGlobalModelHolder.PlayerGlobalModel == null) 
        {
            //first time screen opening in game session

            //TODO: load or initialize player model
            var playerModel = new PlayerGlobalModel(
                DeafultPlayerGlobalModelProvider.PlayerGlobalModel,
                LevelsCollectionProvider.Levels.Length);

            PlayerGlobalModelHolder.SetModel(playerModel);
        }
    }
}
