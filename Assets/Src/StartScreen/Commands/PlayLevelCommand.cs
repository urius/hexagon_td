using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

public class PlayLevelCommand : EventCommand
{
    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher globalDispatcher { get; set; }

    private static bool IsExecutingFlag = false;

    public override async void Execute()
    {
        if (IsExecutingFlag) return;
        IsExecutingFlag = true;
        Retain();        
        
        var tempMusicId = GetMusicId();

        UniTask PreloadMusicAction()
        {
            AudioManager.Instance.Preloadmusic(tempMusicId);
            return UniTask.CompletedTask;
        }

        var transitionHelper = new SwitchScenesWithTransitionSceneHelper(globalDispatcher);
        await transitionHelper.SwitchAsync(SceneNames.MainMenu, SceneNames.Game, PreloadMusicAction);

        Release();
        IsExecutingFlag = false;
    }

    private MusicId GetMusicId()
    {
        var selectedLevelData = PlayerSessionModel.Instance.SelectedLevelData;
        selectedLevelData.MusicId = MusicId.Game_1;
        var tempMusicIds = AudioManager.Instance.GetGameplayMusicIds(selectedLevelData.LevelConfig.DisabedMusicIds);
        if (tempMusicIds.Length > 0)
        {
            var rnd = new Random();
            var musicId = tempMusicIds[rnd.Next(tempMusicIds.Length)];
            selectedLevelData.MusicId = musicId;
        }

        return selectedLevelData.MusicId;
    }
}
