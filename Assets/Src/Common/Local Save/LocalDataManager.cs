using System;
using UnityEngine;

namespace Assets.Src.Common.Local_Save
{
    public class LocalDataManager
    {
        public static readonly LocalDataManager Instance = new();

        private const string PlayerDataSaveKey = "pd";

        public bool TryLoadUserData(out UserDataDto result)
        {
            return TryLoadData(PlayerDataSaveKey, out result);            
        }

        public bool SaveUserData(UserDataDto saveData)
        {
            return TrySaveData(PlayerDataSaveKey, saveData);            
        }

        private bool TrySaveData<T>(string key, T saveData)
        {
            var dataStr = JsonUtility.ToJson(saveData);
            try
            {
                var dataStrEncoded = Base64Helper.Base64Encode(dataStr);
                PlayerPrefs.SetString(key, dataStrEncoded);
            }
            catch (Exception e)
            {
                Debug.LogError($"error while saving data, {e.Message}");
                return false;
            }

            return true;
        }

        private bool TryLoadData<T>(string key, out T result)
            where T : class
        {
            result = null;

            var dataStr = PlayerPrefs.GetString(key);

            if (string.IsNullOrEmpty(dataStr) == false)
            {
                try
                {
                    var dataStrDecoded = Base64Helper.Base64Decode(dataStr);
                    result = JsonUtility.FromJson<T>(dataStrDecoded);
                }
                catch (Exception e)
                {
                    Debug.LogError($"error while deserializing loaded data, {e.Message}");
                    return false;
                }
            }

            return true;
        }
    }
}