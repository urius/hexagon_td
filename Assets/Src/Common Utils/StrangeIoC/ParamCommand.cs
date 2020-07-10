using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;
using UnityEngine;

public abstract class ParamCommand<TParam> : Command
{
    public sealed override void Execute()
    {
        var param = (TParam)(data as TmEvent).data;
        Execute(param);
    }

    public abstract void Execute(TParam data);
}
