using System.Collections.Generic;
using UnityEngine;
using System;
using UICommandSystemSignals;

public class UICommandDispatcher
{
    //외부 의존성
    ISignalHub<IPulicSignal> signalHub;

    public void Initialize(ISignalHub<IPulicSignal> _signalHub)
    {
        signalHub = _signalHub;
    }

    public void Dispatch_CardSystem(in ActionDataBatch_CardSystem _jobBatch)
    {
        if (_jobBatch.actionDataList != null)
            signalHub.Publish(new CardSystem_JobDispatchEvent(_jobBatch));
    }

    public void Release()
    {

    }
}
