using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Transform _rotatableTransform;
    [SerializeField] private Transform _scalableTransform;
    [SerializeField] private SpriteRenderer _colorableSprite;
    [SerializeField] private Color _fullHpColor;
    [SerializeField] private Color _noHpColor;

    public void SetHpPercent(float percent)
    {
        _scalableTransform.transform.localScale = new Vector3(percent, 1, 1);
        _colorableSprite.color = _noHpColor + (_fullHpColor - _noHpColor) * percent;
    }

    private void OnEnable()
    {
        _rotatableTransform.rotation = Camera.main.transform.rotation;

        SetHpPercent(1);
    }
}
