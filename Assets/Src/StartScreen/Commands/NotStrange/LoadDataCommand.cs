using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public struct LoadDataCommand
{
    public async UniTask<bool> ExecuteAsync(string id, RectTransform displayErrorRectTransform)
    {
        var result = await NetworkManager.GetUserDataAsync(id);
        if (result.IsSuccess)
        {
            if (!result.Result.IsError)
            {
                var playerGlobalModel = new PlayerGlobalModel(result.Result.payload);
                var levelsCollectionProvider = LevelsCollectionProvider.Instance;
                playerGlobalModel.AdjustLevelsAmount(levelsCollectionProvider.Levels.Length);
                PlayerGlobalModelHolder.Instance.SetModel(playerGlobalModel);

                return true;
            }
            else
            {
                var localizationProvider = LocalizationProvider.Instance;
                var trayAgainText = localizationProvider.Get(LocalizationGroupId.ErrorPopup, "try_again");
                var errorDescription = localizationProvider.Get(LocalizationGroupId.ErrorPopup, "retreive_data_error_description");
                errorDescription = string.Format(errorDescription, result.Result.error.code);
                var errorPopup = ErrorPopup.Show(displayErrorRectTransform, errorDescription, trayAgainText);

                await errorPopup.LifeTimeTask;
            }
        }
        else
        {
            var localizationProvider = LocalizationProvider.Instance;
            var trayAgainText = localizationProvider.Get(LocalizationGroupId.ErrorPopup, "try_again");
            var errorDescription = localizationProvider.Get(LocalizationGroupId.ErrorPopup, "connection_error_description");
            var errorPopup = ErrorPopup.Show(displayErrorRectTransform, errorDescription, trayAgainText);

            await errorPopup.LifeTimeTask;
        }

        return false;
    }
}
