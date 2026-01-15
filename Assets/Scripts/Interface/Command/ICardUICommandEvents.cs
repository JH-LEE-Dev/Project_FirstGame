using System;
using UnityEngine;
using System.Collections.Generic;

public interface ICardUICommandEvents
{
    public event Action<UIJobBatch_CardSystem> JobDispatchEvent;
}
