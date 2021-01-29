using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

    public async static Task<WebRequestResult<PlayerDataResponsePayload>> GetUserRemoteDataAsync(string id)
    {
        var requestResult = await WebRequestsSender.GetAsync<NetworkDefaultResponse<PlayerDataResponsePayload>>($"{DataProviderUrl}?command=get&id={id}");
        if (requestResult.IsSuccess)
        {
            if (requestResult.Result.error.code == 0)
            {
                return new WebRequestResult<PlayerDataResponsePayload>(requestResult.Result.payload);
            }
        }
        return new WebRequestResult<PlayerDataResponsePayload>();
    }
}

[Serializable]
public struct NetworkDefaultResponse<TPayload>
{
    public TPayload payload;
    public WebRequestError error;
}

[Serializable]
public struct PlayerDataResponsePayload
{
    public LevelProgressDataMin[] LevelsProgress;
}

[Serializable]
public struct WebRequestError
{
    public int code;
    public string message;
}
