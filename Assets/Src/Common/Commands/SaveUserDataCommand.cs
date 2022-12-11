using Assets.Src.Common.Local_Save;

public struct SaveUserDataCommand
{
    public bool Execute()
    {
        var model = PlayerSessionModel.Model;
        var result = LocalDataManager.Instance.SaveUserData(model.ToSaveDto());
        return result;
    }
}
