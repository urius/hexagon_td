public class SelectLevelClickedCommand : ParamCommand<int>
{
    public override void Execute(int levelIndex)
    {
        PlayerSessionModel.Instance.SelectedLevelData.LevelIndex = levelIndex;
    }
}
