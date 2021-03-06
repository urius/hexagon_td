using System;
using strange.extensions.command.impl;

public class StartLevelCommand : Command
{
    [Inject] public LevelModel LevelModel { get; set; }

    public override void Execute()
    {
        injectionBinder.GetInstance<ProcessUpdatesSystem>().Start();
        injectionBinder.GetInstance<UnitsControlSystem>().Start();
        injectionBinder.GetInstance<BulletsHitSystem>().Start();
        injectionBinder.GetInstance<TurretsControlSystem>().Start();
        injectionBinder.GetInstance<WavesControlSystem>().Start();
        injectionBinder.GetInstance<PlayerDataUpdateSystem>().Start();

        LevelModel.SetLevelStarted();

        var tempMusicIds = AudioManager.Instance.GetGameplayMusicIds(LevelModel.LevelConfig.DisabedMusicIds);
        if (tempMusicIds.Length > 0)
        {
            var rnd = new Random();
            var musicId = tempMusicIds[rnd.Next(tempMusicIds.Length)];

            AudioManager.Instance.FadeInAndPlayMusicIfNotPlayedAsync(musicId);
        }
    }
}
