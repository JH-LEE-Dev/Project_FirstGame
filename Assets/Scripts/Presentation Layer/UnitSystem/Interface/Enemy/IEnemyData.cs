using UnityEngine;
using System;
using System.Collections.Generic;

public interface IEnemyData
{
    int enemyID { get; }
    Transform GetTransform();
    float GetMaxHealth();
    float GetCurrentHealth();
    IHealthComponentProvider healthComponentProvider { get; }
    IEnemyStatProvider enemyStatProvider { get; }
    bool bDead { get; }
    event Action EnemySpawnedEvent;
    event Action EnemyIsDeadEvent;
    EnemyTypeData enemyTypeData { get; }
    IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> currentAppliedDebuff { get; }
    event Action EnemyDebuffChangedEvent;
}
