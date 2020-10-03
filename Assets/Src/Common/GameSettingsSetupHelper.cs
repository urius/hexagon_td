using DG.Tweening;
using UnityEngine;

public class GameSettingsSetupHelper
{
    public static void Setup(int fps = 30)
    {
        Application.targetFrameRate = fps;

        DOTween.Init(true, false, LogBehaviour.ErrorsOnly).SetCapacity(50, 0);
        DOTween.defaultEaseType = Ease.Linear;
    }
}
