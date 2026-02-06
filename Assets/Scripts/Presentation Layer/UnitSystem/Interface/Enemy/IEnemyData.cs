using UnityEngine;
using System;

public interface IEnemyData
{
    Transform GetTransform();
    float GetMaxHealth();
    float GetCurrentHealth();
    IHealthComponentProvider healthComponentProvider { get; }
    IEnemyStatProvider enemyStatProvider { get; }
    event Action EnemySpawnedEvent;
    event Action EnemyIsDeadEvent;
}
