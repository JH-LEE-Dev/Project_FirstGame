using System;
using UnityEngine;
using System.Collections.Generic;

public interface ICardUICommandEvents
{
    event Action<List<JobType_CardSystemUI>> JobDispatchEvent;
}
