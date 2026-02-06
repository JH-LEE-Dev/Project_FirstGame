using UnityEngine;
using System;

public interface IEnemyData
{
    Transform GetTransform();
    float GetMaxHealth();
    float GetCurrentHealth();
    IHealthComponentProvider healthComponentProvider { get; }
    event Action EnemySpawnedEvent;
}
