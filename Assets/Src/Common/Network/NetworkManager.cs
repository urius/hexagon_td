using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager
{
    private const string DataProviderUrl = "https://twinpixel.ru/data_provider.php";

    private static WebRequestsSender _webRequestsSender;
    private static WebRequestsSender WebRequestsSender
    {
        get
        {
            if (_webRequestsSender == null)
            {
                _webRequestsSender = new WebRequestsSender();
            }
            return _webRequestsSender;
        }
    }

    public async static Task<WebRequestResult<NetworkDefaultResponse<PlayerGlobalModel>>> GetUserDataAsync(string id)
    {
        var requestResult = await WebRequestsSender.GetAsync<NetworkDefaultResponse<PlayerGlobalModel>>($"{DataProviderUrl}?command=get&id={id}");
        return requestResult;
    }

    public async static Task<WebRequestResult<NetworkDefaultResponse<SaveDataResponsePayload>>> SaveUserDataAsync(PlayerGlobalModel model)
    {
        var id = model.Id;
        var saveData = ConvertToSavePlayerDataRequest(model);
        var saveDataStr = JsonUtility.ToJson(saveData);
        var requestResult = await WebRequestsSender.PostAsync<NetworkDefaultResponse<SaveDataResponsePayload>>($"{DataProviderUrl}?command=set&id={id}", saveDataStr);
        return requestResult;
    }

    public async static Task<WebRequestResult<NetworkDefaultResponse<SaveDataResponsePayload>>> SaveUserAudioSettingsAsync(PlayerGlobalModel model)
    {
        var id = model.Id;
        var saveData = ConvertToSavePlayerAudioSettingsRequest(model);
        var saveDataStr = JsonUtility.ToJson(saveData);
        var requestResult = await WebRequestsSender.PostAsync<NetworkDefaultResponse<SaveDataResponsePayload>>($"{DataProviderUrl}?command=set&id={id}", saveDataStr);
        return requestResult;
    }

    private static SavePlayerDataRequest ConvertToSavePlayerDataRequest(PlayerGlobalModel model)
    {
        return new SavePlayerDataRequest()
        {
            LoadsCount = model.LoadsCount,
            LevelsProgress = model.LevelsProgress,
            AudioVolume = model.AudioVolume,
            MusicVolume = model.MusicVolume,
            SoundsVolume = model.SoundsVolume,
        };
    }

    private static SavePlayerAudioSettingsRequest ConvertToSavePlayerAudioSettingsRequest(PlayerGlobalModel model)
    {
        return new SavePlayerAudioSettingsRequest()
        {
            AudioVolume = model.AudioVolume,
            MusicVolume = model.MusicVolume,
            SoundsVolume = model.SoundsVolume,
        };
    }
}

[Serializable]
public struct NetworkDefaultResponse<TPayload>
{
    public TPayload payload;
    public WebRequestError error;

    public bool IsError => error.code != 0;
}

[Serializable]
public struct WebRequestError
{
    public int code;
    public string message;
}

[Serializable]
public struct SaveDataResponsePayload
{
    public bool result;
}

[Serializable]
public struct SavePlayerDataRequest
{
    public int LoadsCount;
    public LevelProgressDataMin[] LevelsProgress;
    public float AudioVolume;
    public float MusicVolume;
    public float SoundsVolume;
}

[Serializable]
public struct SavePlayerAudioSettingsRequest
{
    public float AudioVolume;
    public float MusicVolume;
    public float SoundsVolume;
}
