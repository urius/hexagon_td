public class SelectLevelClickedCommand : ParamCommand<int>
{
    [Inject] public LevelConfigProvider LevelConfigProvider { get; set; }
    [Inject] public LevelsCollectionProvider LevelsCollectionProvider { get; set; }

    public override void Execute(int levelIndex)
    {
        LevelConfigProvider.SetCurrentLevelConfig(LevelsCollectionProvider.Levels[levelIndex]);
    }
}
