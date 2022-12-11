using Cysharp.Threading.Tasks;
public struct SaveSettingsCommand
{
    public bool Execute()
    {
        return new SaveUserDataCommand().Execute();
    }
}
