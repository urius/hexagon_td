using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestsSender
{
    public async UniTask<WebRequestResult<T>> GetAsync<T>(string url)
    {
        var resultStr = await GetAsync(url);
        if (resultStr.IsSuccess)
        {
            T result;
            try
            {
                result = JsonUtility.FromJson<T>(resultStr.Result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new WebRequestResult<T>();
            }
            return new WebRequestResult<T>(result);
        }
        else
        {
            return new WebRequestResult<T>();
        }
    }

    public async UniTask<WebRequestResult<string>> GetAsync(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var result = await request.SendWebRequest(); // TODO: try/catch
            Debug.Log($"Request -> {url}\n Response -> {result.downloadHandler.text}");
            return new WebRequestResult<string>(result.downloadHandler.text);
        }
    }
}

public struct WebRequestResult<T>
{
    public WebRequestResult(T result)
    {
        IsSuccess = true;
        Result = result;
    }

    public bool IsSuccess;
    public T Result;
}
