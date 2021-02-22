using Cysharp.Threading.Tasks;

public struct SaveSettingsCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var model = PlayerSessionModel.Model;
        var result = await NetworkManager.SaveUserAudioSettingsAsync(model.Id, model.ToSaveSettingsDto());
        return result.IsSuccess;
    }
}
