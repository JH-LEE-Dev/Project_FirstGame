using System.Collections.Generic;
using UnityEngine;
using System;
using UICommandSystemSignals;

public class UICommandDispatcher
{
    //외부 의존성
    SignalHub signalHub;

    public void Initialize(SignalHub _signalHub)
    {
        signalHub = _signalHub;
    }

    public void Dispatch_CardSystem(in CardUIActionBatch _jobBatch)
    {
        if (_jobBatch.actionList != null)
            signalHub.Publish(new CardSystem_ActionDispatchSignal(_jobBatch));
    }

    public void Release()
    {

    }
}
