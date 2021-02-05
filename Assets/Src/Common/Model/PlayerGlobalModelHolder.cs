using UnityEngine;

[CreateAssetMenu(fileName = "PlayerGlobalModelHolder", menuName = "Common/Model/PlayerGlobalModelHolder")]
public class PlayerGlobalModelHolder : ScriptableObject
{
    public static PlayerGlobalModelHolder Instance;
    public static PlayerGlobalModel Model => Instance.PlayerGlobalModel;

    [SerializeField]
    private PlayerGlobalModel _playerGlobalModel;
    public PlayerGlobalModel PlayerGlobalModel => _playerGlobalModel;

    private void OnEnable()
    {
        Instance = this;
    }

    public void SetModel(PlayerGlobalModel model)
    {
        _playerGlobalModel = model;
    }
}
