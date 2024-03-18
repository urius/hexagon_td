using Cysharp.Threading.Tasks;

public struct SaveSettingsCommand
{
    public UniTask<bool> ExecuteAsync()
    {
        new SaveUserDataCommand().Execute();
        
        return UniTask.FromResult(true);
    }
}
