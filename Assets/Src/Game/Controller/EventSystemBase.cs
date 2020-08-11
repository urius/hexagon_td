using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

public abstract class EventSystemBase
{
    [Inject(ContextKeys.CONTEXT_DISPATCHER)]
    public IEventDispatcher dispatcher { get; set; }

    public EventSystemBase()
    {
    }
}
