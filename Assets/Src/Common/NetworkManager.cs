using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class NetworkManager
{
    public async static Task<T> GetAsync<T>(string url)
    {
        var sender = new WebRequestsSender();
        var response = await sender.GetAsync<NetworkDefaultResponse<T>>(url);
        return response.payload;
    }
}

[Serializable]
public class NetworkDefaultResponse<TPayload>
{
    public TPayload payload;
    public WebRequestError error;
}

[Serializable]
public class WebRequestError
{
    public int code;
    public string message;
}
