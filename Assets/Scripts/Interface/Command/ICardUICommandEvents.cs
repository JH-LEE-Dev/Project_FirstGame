using System;
using UnityEngine;
using System.Collections.Generic;

public interface ICardUICommandEvents
{
    public event Action<List<Job_CardSystemUI>> JobDispatchEvent;
}
