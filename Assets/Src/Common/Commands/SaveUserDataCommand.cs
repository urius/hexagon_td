using Cysharp.Threading.Tasks;

public struct SaveUserDataCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var model = PlayerSessionModel.Model;
        var result = await NetworkManager.SaveUserDataAsync(model.Id, model.ToSaveDto());
        return result.IsSuccess;
    }
}
