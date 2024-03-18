using UnityEngine;

namespace Src.Common.Commands
{
    public struct SaveUserModelCommand
    {
        public void Execute(PlayerGlobalModel model)
        {
            var saveDto = model.ToSaveDto();
            var saveJson = JsonUtility.ToJson(saveDto);
        
            PlayerPrefs.SetString(Constants.SavedDataKey, saveJson);
            PlayerPrefs.Save();
        }
    }
}