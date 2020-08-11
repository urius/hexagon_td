using strange.extensions.command.impl;

public class StartLevelCommand : Command
{
    [Inject] public LevelModel levelModel { get; set; }

    public override void Execute()
    {
        levelModel.IsInitialized = true;

        injectionBinder.GetInstance<ProcessUpdatesSystem>().Start();
        injectionBinder.GetInstance<UnitsControlSystem>().Start();
    }
}
