using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public struct SaveUserDataCommand
{
    public async UniTask<bool> ExecuteAsync(PlayerGlobalModel model)
    {
        var result = await NetworkManager.SaveUserDataAsync(model.Id, model.ToSaveDto());
        return result.IsSuccess;
    }
}
