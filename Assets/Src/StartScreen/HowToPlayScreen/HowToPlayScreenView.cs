using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayScreenView : ScreenView
{
    [SerializeField] private Button _homeButton;
    public Button HomeButton => _homeButton;
    [SerializeField] private Button _leftArrowButton;
    public Button LeftArrowButton => _leftArrowButton;
    [SerializeField] private Button _rightArrowButton;
    public Button RightArrowButton => _rightArrowButton;
    [SerializeField] private Button _nextButton;
    public Button NextButton => _nextButton;
    [SerializeField] private CanvasGroup[] _slideCanvasGroups;
    public CanvasGroup[] SlideCanvasGroups => _slideCanvasGroups;
}
