using Src.StartScreen.Commands.NotStrange;

public struct LoadDataCommand
{
    public void Execute()
    {
        var playerGlobalModelDto = new GetPlayerGlobalModelDtoCommand().Execute();

        var playerGlobalModel = new PlayerGlobalModel(playerGlobalModelDto);
        var levelsCount = LevelsCollectionProvider.Instance.Levels.Length;
        playerGlobalModel.AdjustLevelsAmount(levelsCount);
        
        PlayerSessionModel.Instance.SetModel(playerGlobalModel);
    }
}
