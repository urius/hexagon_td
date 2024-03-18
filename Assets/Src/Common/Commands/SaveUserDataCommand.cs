using Src.Common.Commands;

public struct SaveUserDataCommand
{
    public void Execute()
    {
        new SaveUserModelCommand().Execute(PlayerSessionModel.Model);
    }
}
