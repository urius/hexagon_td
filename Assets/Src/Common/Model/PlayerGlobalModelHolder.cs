using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "PlayerGlobalModelHolder", menuName = "Common/Model/PlayerGlobalModelHolder")]
public class PlayerGlobalModelHolder : ScriptableObject
{
    [SerializeField]
    private PlayerGlobalModel _playerGlobalModel;
    public PlayerGlobalModel PlayerGlobalModel => _playerGlobalModel;

    private TaskCompletionSource<bool> _modelInnitializedTsc = new TaskCompletionSource<bool>();
    public Task ModelInnitializedTask => _modelInnitializedTsc.Task;

    public void SetModel(PlayerGlobalModel model)
    {
        _playerGlobalModel = model;
        _modelInnitializedTsc.TrySetResult(true);
    }

    public async void Load()
    {
        var result = await NetworkManager.Instance.GetAsync("https://twinpixel.ru/data_provider.php?command=get&id=test4");
        Debug.Log("Download complete");
        Debug.Log(result.downloadHandler.text);
    }

    public void Save()
    {

    }
}
