using UnityEngine;
using System;

public interface IHealthComponentProvider
{
    event Action TakeDamageEvent;

    float maxHealth { get; }
    float currentHealth { get; }
    float currentShield { get; }
    float prevHealth { get; }
    float prevShield { get; }
    bool bWeakness { get; }
}
