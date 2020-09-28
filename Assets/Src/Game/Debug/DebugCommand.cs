using System;
public class DebugCommand : ParamCommand<int>
{
    [Inject] public LevelModel LevelModel { get; set; }

    public override void Execute(int data)
    {
        LevelModel.ResetLevel();
    }
}
