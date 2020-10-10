using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsScrollItemSelectionView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _unlockedModeSprite;
    [SerializeField] private Color _unlockedModeColor;
    [SerializeField] private Sprite _lockedModeSprite;
    [SerializeField] private Color _lockedModeColor;

    public void SetLockedState(bool isLocked)
    {
        _image.sprite = isLocked ? _lockedModeSprite : _unlockedModeSprite;
        _image.color = isLocked ? _lockedModeColor : _unlockedModeColor;
    }

    private void Update()
    {
        transform.Rotate(transform.forward, -3);
    }
}
