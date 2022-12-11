using UnityEngine;
using Cysharp.Threading.Tasks;
using Assets.Src.Common.Local_Save;

public struct LoadDataCommand
{
    public async UniTask<bool> ExecuteAsync(RectTransform displayErrorRectTransform)
    {
        var isLoaded = LocalDataManager.Instance.TryLoadUserData(out var userDataDto);

        if (isLoaded)
        {
            var levelsCollectionProvider = LevelsCollectionProvider.Instance;

            if (userDataDto == null)
            {
                userDataDto = GetDefaultPlayerDataDto();
            }

            var playerGlobalModel = new PlayerGlobalModel(userDataDto);
            playerGlobalModel.AdjustLevelsAmount(levelsCollectionProvider.Levels.Length);
            PlayerSessionModel.Instance.SetModel(playerGlobalModel);

            return true;
        }
        else
        {
            var localizationProvider = LocalizationProvider.Instance;
            var trayAgainText = localizationProvider.Get(LocalizationGroupId.ErrorPopup, "try_again");
            var errorDescription = localizationProvider.Get(LocalizationGroupId.ErrorPopup, "retreive_data_error_description");
            errorDescription = string.Format(errorDescription, 0);
            var errorPopup = ErrorPopup.Show(displayErrorRectTransform, errorDescription, trayAgainText);

            await errorPopup.LifeTimeTask;
        }

        return false;
    }

    private UserDataDto GetDefaultPlayerDataDto()
    {
        return new UserDataDto
        {
            loads = 0,
            levels_progress = new LevelProgressDto[0],
            settings = new PlayerAudioSettingsDto { audio = 0.5f, music = 0.5f, sounds = 0.5f },
            gold_str = Base64Helper.Base64Encode(500.ToString())
        };
    }
}
