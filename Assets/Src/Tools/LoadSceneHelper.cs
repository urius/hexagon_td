using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneHelper : MonoBehaviour
{
    public static UniTask<Scene> LoadSceneAdditiveAsync(string sceneName, out AsyncOperation asyncOperation)
    {
        var _tsc = new UniTaskCompletionSource<Scene>();
        asyncOperation = null;
        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == sceneName)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                _tsc.TrySetResult(scene);
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        return _tsc.Task;
    }

    public static UniTask<Scene> LoadSceneAdditiveAsync(string sceneName)
    {
        return LoadSceneAdditiveAsync(sceneName, out var _);
    }

    public static UniTask UnloadSceneAsync(string sceneName)
    {
        var _tsc = new UniTaskCompletionSource<bool>();

        void OnSceneUnloaded(Scene scene)
        {
            if (scene.name == sceneName)
            {
                SceneManager.sceneUnloaded -= OnSceneUnloaded;
                _tsc.TrySetResult(true);
            }
        }

        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.None);

        return _tsc.Task;
    }
}
