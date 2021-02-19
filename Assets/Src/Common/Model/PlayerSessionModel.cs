using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSessionModel", menuName = "Common/Model/PlayerSessionModel")]
public class PlayerSessionModel : ScriptableObject
{
    public static PlayerSessionModel Instance { get; private set; }
    public static PlayerGlobalModel Model => Instance.PlayerGlobalModel;

    [SerializeField]
    private PlayerGlobalModel _playerGlobalModel;
    public PlayerGlobalModel PlayerGlobalModel => _playerGlobalModel;

    public int SelectedLevelIndex = -1;
    public LevelConfig SelectedLevelConfig => SelectedLevelIndex >= 0 ? LevelsCollectionProvider.Instance.Levels[SelectedLevelIndex] : null;

    public void Reset()
    {
        SelectedLevelIndex = -1;
    }

    public void SetModel(PlayerGlobalModel model)
    {
        _playerGlobalModel = model;
    }

    public void AdvanceSelectedLevel()
    {
        SelectedLevelIndex++;
        var totalLevelsCount = LevelsCollectionProvider.Instance.Levels.Length;
        if (SelectedLevelIndex > totalLevelsCount - 1)
        {
            SelectedLevelIndex = 0;
        }
    }

    private void OnEnable()
    {
        Instance = this;
    }
}
