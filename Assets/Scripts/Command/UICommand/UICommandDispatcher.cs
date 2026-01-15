using System.Collections.Generic;
using UnityEngine;
using System;

public class UICommandDispatcher
{
    public event Action<UIJobBatch_CardSystem> CardSystem_JobDispatchEvent;

    public void Dispatch_CardSystem(in UIJobBatch_CardSystem _jobBatch)
    {
        if (_jobBatch.jobList != null)
            CardSystem_JobDispatchEvent?.Invoke(_jobBatch);
    }

    public void Release()
    {
        CardSystem_JobDispatchEvent = null;
    }
}
