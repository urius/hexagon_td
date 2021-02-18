using System;
using System.Threading.Tasks;
using UnityEngine;

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

    public async static Task<WebRequestResult<NetworkDefaultResponse<GetUserDataResponse>>> GetUserDataAsync(string id)
    {
        var requestResult = await WebRequestsSender.GetAsync<NetworkDefaultResponse<GetUserDataResponse>>($"{DataProviderUrl}?command=get&id={id}");
        return requestResult;
    }

    public async static Task<WebRequestResult<NetworkDefaultResponse<GetProductsResponse>>> GetStoreProductsAsync()
    {
        var requestResult = await WebRequestsSender.GetAsync<NetworkDefaultResponse<GetProductsResponse>>($"{DataProviderUrl}?command=get_products");
        return requestResult;
    }

    public async static Task<WebRequestResult<NetworkDefaultResponse<SaveDataResponsePayload>>> SaveAsync<TData>(string id, TData saveData)
        where TData : struct
    {
        var saveDataStr = JsonUtility.ToJson(saveData);
        var requestResult = await WebRequestsSender.PostAsync<NetworkDefaultResponse<SaveDataResponsePayload>>($"{DataProviderUrl}?command=set&id={id}", saveDataStr);
        return requestResult;
    }

    public static Task<WebRequestResult<NetworkDefaultResponse<SaveDataResponsePayload>>> SaveUserDataAsync(string id, SavePlayerDataRequest saveData)
    {
        return SaveAsync(id, saveData);
    }

    public static Task<WebRequestResult<NetworkDefaultResponse<SaveDataResponsePayload>>> SaveUserAudioSettingsAsync(string id, SavePlayerSettingsRequest saveData)
    {
        return SaveAsync(id, saveData);
    }

    public static Task<WebRequestResult<NetworkDefaultResponse<SaveDataResponsePayload>>> SaveUserGoldAsync(string id, int goldAmount)
    {
        var saveData = ConvertToSavePlayerGoldRequest(goldAmount);
        return SaveAsync(id, saveData);
    }

    private static SavePlayerGoldRequest ConvertToSavePlayerGoldRequest(int goldAmount)
    {
        return new SavePlayerGoldRequest()
        {
            gold = goldAmount,
            gold_str = Base64Helper.Base64Encode(goldAmount.ToString()),
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
public struct GetProductsResponse
{
    public string[] products;
}

[Serializable]
public struct PlayerAudioSettingsDto
{
    public float audio;
    public float music;
    public float sounds;
}

[Serializable]
public struct GetUserDataResponse
{
    public string id;
    public int loads;
    public LevelProgressDto[] levels_progress;
    public PlayerAudioSettingsDto settings;
    public string gold_str;
}

[Serializable]
public struct LevelProgressDto
{
    public bool is_passed;
    public bool is_unlocked;
    public int stars_amount;
}

[Serializable]
public struct SaveDataResponsePayload
{
    public bool result;
}

[Serializable]
public struct SavePlayerDataRequest
{
    public int loads;
    public LevelProgressDto[] levels_progress;
    public PlayerAudioSettingsDto settings;
    public string gold_str;
}

[Serializable]
public struct SavePlayerSettingsRequest
{
    public PlayerAudioSettingsDto settings;
}

[Serializable]
public struct SavePlayerGoldRequest
{
    public int gold;
    public string gold_str;
}
