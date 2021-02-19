using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSessionModel", menuName = "Common/Model/PlayerSessionModel")]
public class PlayerSessionModel : ScriptableObject
{
    public static PlayerSessionModel Instance;
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
