using UnityEngine;
using System;
using System.Collections.Generic;

public interface IEnemyData
{
    Transform GetTransform();
    float GetMaxHealth();
    float GetCurrentHealth();
    IHealthComponentProvider healthComponentProvider { get; }
    IEnemyStatProvider enemyStatProvider { get; }
    event Action EnemySpawnedEvent;
    event Action EnemyIsDeadEvent;
    EnemyTypeData enemyTypeData { get; }
    IReadOnlyDictionary<DebuffElementEffectType,DebuffElementData> currentAppliedDebuff { get; }
}
