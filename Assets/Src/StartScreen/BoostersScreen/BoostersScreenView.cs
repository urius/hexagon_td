using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostersScreenView : ScreenView
{
    [SerializeField] private Text _levelTitleText;
    [SerializeField] private Text _boostersTitleText;
    [SerializeField] private RectTransform _contentContainer;
    [SerializeField] private Button _playButton;
}
