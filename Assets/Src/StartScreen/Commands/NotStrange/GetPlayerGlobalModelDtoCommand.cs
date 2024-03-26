using Src.Common;
using Src.Common.Dtos;
using UnityEngine;

namespace Src.StartScreen.Commands.NotStrange
{
    public struct GetPlayerGlobalModelDtoCommand
    {
        public SavePlayerDataDto Execute()
        {
            var savedDataStr = PlayerPrefs.GetString(Constants.SavedDataKey, null);

            return string.IsNullOrEmpty(savedDataStr)
                ? new PlayerGlobalModel(GenerateId()).ToSaveDto()
                : JsonUtility.FromJson<SavePlayerDataDto>(savedDataStr);
        }

        private static string GenerateId()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
    }
}