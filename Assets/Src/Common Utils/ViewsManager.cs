using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IViewManager
{
    GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation);
    bool Destroy(GameObject instance);
}

public class ViewsManager : IViewManager
{
    private readonly Dictionary<GameObject, GameObject> _prefabByInstanceMap = new Dictionary<GameObject, GameObject>();
    private readonly Dictionary<GameObject, GameObjectsPool> _cacheByPrefabMap = new Dictionary<GameObject, GameObjectsPool>();

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!_cacheByPrefabMap.TryGetValue(prefab, out var cache))
        {
            cache = new GameObjectsPool(prefab);
            _cacheByPrefabMap[prefab] = cache;
        }

        var instance = cache.Get(position, rotation);
        _prefabByInstanceMap[instance] = prefab;

        return instance;
    }

    public bool Destroy(GameObject instance)
    {
        if (_prefabByInstanceMap.TryGetValue(instance, out var prefab)
            && _cacheByPrefabMap.TryGetValue(prefab, out var cache))
        {
            return cache.Return(instance);
        }

        Debug.Log("Unable to destroy GameObject: " + instance);
        Debug.Break();

        return false;
    }
}

public class GameObjectsPool
{
    private readonly GameObject _prefab;
    private readonly IList<GameObject> _views = new List<GameObject>();

    public GameObjectsPool(GameObject prefab)
    {
        _prefab = prefab;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        if (_views.Any(v => v == null))
        {
            Debug.Break();
        }

        var view = _views.FirstOrDefault(v => !v.activeSelf);
        if (view == null)
        {
            view = GameObject.Instantiate(_prefab, position, rotation);
            _views.Add(view);
        }
        else
        {
            view.transform.position = position;
            view.transform.rotation = rotation;
            view.SetActive(true);
        }

        return view;
    }

    public bool Return(GameObject gameObject)
    {
        var view = _views.FirstOrDefault(v => v == gameObject);
        view?.SetActive(false);

        return view != null;
    }
}
