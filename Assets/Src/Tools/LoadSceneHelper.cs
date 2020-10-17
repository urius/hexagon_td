using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneHelper : MonoBehaviour
{
    public static Task<Scene> LoadSceneAdditiveAsync(string sceneName, out AsyncOperation asyncOperation)
    {
        var _tsc = new TaskCompletionSource<Scene>();
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

    public static Task<Scene> LoadSceneAdditiveAsync(string sceneName)
    {
        return LoadSceneAdditiveAsync(sceneName, out var _);
    }

    public static Task UnloadSceneAsync(string sceneName)
    {
        var _tsc = new TaskCompletionSource<bool>();

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
