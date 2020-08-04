using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    public abstract void Setup(UnitView targetView, UnitModel targetModel, IEventDispatcher dispatcher, IViewManager viewManager);
}
