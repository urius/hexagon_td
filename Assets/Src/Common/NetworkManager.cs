using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    public Task<UnityWebRequest> GetAsync(string url)
    {
        var tsc = new TaskCompletionSource<UnityWebRequest>();
        void Callback(UnityWebRequest request)
        {
            tsc.TrySetResult(request);
        }

        StartCoroutine(GetRequest(url, Callback));

        return tsc.Task;
    }

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator GetRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            callback(request);
        }
    }
}
