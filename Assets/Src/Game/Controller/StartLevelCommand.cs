﻿using strange.extensions.command.impl;

public class StartLevelCommand : Command
{
    [Inject] public LevelModel LevelModel { get; set; }

    public override void Execute()
    {
        injectionBinder.GetInstance<ProcessUpdatesSystem>().Start();
        injectionBinder.GetInstance<UnitsControlSystem>().Start();
        injectionBinder.GetInstance<BulletsHitSystem>().Start();
        injectionBinder.GetInstance<TurretsControlSystem>().Start();
        injectionBinder.GetInstance<WavesControlSystem>().Start();

        injectionBinder.GetInstance<ShowPathsMediator>();

        LevelModel.SetLevelStarted();
    }
}
