using System.Collections.Generic;
using UnityEngine;
using System;

public class UICommandDispatcher 
{
    public event Action<List<Job_CardSystemUI>> CardSystem_JobDispatchEvent;
    public void Dispatch_CardSystem(List<Job_CardSystemUI> JobBatch)
    {
        CardSystem_JobDispatchEvent?.Invoke(JobBatch);
    }
}
