using TMPro;
using UnityEngine;

public class GoalBaseView : CellView
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private Color _colorLiteDamage;
    [SerializeField] private Color _colorCritical;

    public void SetGoalNum(int number)
    {
        _text.text = number.ToString();
    }

    public void SetColorMode(GoalBaseColor color)
    {
        switch (color)
        {
            case GoalBaseColor.LiteDamage:
                _text.color = _colorLiteDamage;
                break;
            case GoalBaseColor.CriticalDamage:
                _text.color = _colorCritical;
                break;
        }
    }
}

public enum GoalBaseColor
{
    Default,
    LiteDamage,
    CriticalDamage,
}
