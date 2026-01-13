using System;
using UnityEngine;

public interface IUnitEvent
{
    event Action<float> TakeDamageEvent;
}
