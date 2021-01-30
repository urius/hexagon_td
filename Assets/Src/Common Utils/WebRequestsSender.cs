using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestsSender
{
    public async UniTask<WebRequestResult<T>> GetAsync<T>(string url)
    {
        var resultStr = await GetAsync(url);
        return HandleGenericResponse<T>(resultStr);
    }

    public UniTask<WebRequestResult<string>> GetAsync(string url)
    {
        return SendRequestAsync(UnityWebRequest.Get(url));
    }

    public async UniTask<WebRequestResult<T>> PostAsync<T>(string url, string postData)
    {
        var resultStr = await PostAsync(url, postData);
        return HandleGenericResponse<T>(resultStr);
    }

    public UniTask<WebRequestResult<string>> PostAsync(string url, string postData)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", postData);
        var request = UnityWebRequest.Post(url, form);

        return SendRequestAsync(request);
    }

    private async UniTask<WebRequestResult<string>> SendRequestAsync(UnityWebRequest unityWebRequest)
    {
        using (unityWebRequest)
        {
            try
            {
                var result = await unityWebRequest.SendWebRequest();

                Debug.Log($"[ Request ] -> {unityWebRequest.url}\n[ Response ] -> {result.downloadHandler.text}");
                if (result.error == null)
                {
                    return new WebRequestResult<string>(result.downloadHandler.text);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        return new WebRequestResult<string>();
    }

    private WebRequestResult<T> HandleGenericResponse<T>(WebRequestResult<string> resultStr)
    {
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
