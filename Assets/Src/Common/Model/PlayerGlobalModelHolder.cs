using UnityEngine;

[CreateAssetMenu(fileName = "PlayerGlobalModelHolder", menuName = "Common/Model/PlayerGlobalModelHolder")]
public class PlayerGlobalModelHolder : ScriptableObject
{
    [SerializeField]
    private PlayerGlobalModel _playerGlobalModel;
    public PlayerGlobalModel PlayerGlobalModel => _playerGlobalModel;

    public void SetModel(PlayerGlobalModel model)
    {
        _playerGlobalModel = model;
    }
}
