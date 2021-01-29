using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestsSender
{
    public async UniTask<T> GetAsync<T>(string url)
    {
        var result = await GetAsync(url); // TODO: try/catch
        return JsonUtility.FromJson<T>(result);
    }

    public async UniTask<string> GetAsync(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var result = await request.SendWebRequest(); // TODO: try/catch
            Debug.Log($"Request -> {url}\n Response -> {result.downloadHandler.text}");
            return result.downloadHandler.text;
        }
    }
}
