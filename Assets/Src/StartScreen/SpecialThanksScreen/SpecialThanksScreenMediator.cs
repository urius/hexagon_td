using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class SpecialThanksScreenMediator : EventMediator
{
    [Inject] public SpecialThanksScreenView SpecialThanksScreenView { get; set; }

}
