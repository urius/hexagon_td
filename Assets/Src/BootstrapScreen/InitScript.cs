using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitScript : MonoBehaviour
{
    [SerializeField] private PlayerSessionModel _playerGlobalModelHolder;
    [SerializeField] private Text _loadingText;
    [SerializeField] private RectTransform _canvasTransform;

    private void Start()
    {
        _loadingText.text = LocalizationProvider.Instance.Get(LocalizationGroupId.BootstrapScreen, "loading");

        PlayerSessionModel.Instance.Reset();

        StartLoadSequence().Forget();
    }

    private async UniTaskVoid StartLoadSequence()
    {
        if (await LoadOrCreateData())
        {
            SetupAudioManager();
            LoadScene();
        }
        else
        {
            await UniTask.DelayFrame(10);

            if (Application.isPlaying)
            {
                StartLoadSequence().Forget();
            }        
        }
    }

    private void SetupAudioManager()
    {
        var playerModel = _playerGlobalModelHolder.PlayerGlobalModel;
        AudioManager.Instance.SetMasterVolume(playerModel.AudioVolume);
        AudioManager.Instance.SetMusicVolume(playerModel.MusicVolume);
        AudioManager.Instance.SetSoundsVolume(playerModel.SoundsVolume);
    }

    private async UniTask<bool> LoadOrCreateData()
    {
        _loadingText.text = "load data";

        var id = SystemInfo.deviceUniqueIdentifier;
        var result = await new LoadDataCommand().ExecuteAsync(_canvasTransform);

        return result;
    }

    private async void LoadScene()
    {
        _loadingText.text = "load scene";

        await LoadSceneHelper.LoadSceneAdditiveAsync(SceneNames.MainMenu);
        await SceneManager.UnloadSceneAsync(SceneNames.Bootstrap, UnloadSceneOptions.None);
    }
}
