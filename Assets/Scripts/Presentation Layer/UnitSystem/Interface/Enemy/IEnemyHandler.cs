using UnityEngine;
using System;
using System.Collections.Generic;

public interface IEnemyHandler
{
    bool bDead { get; }
    Transform GetTransform();
    float GetMaxHealth();
    float GetCurrentHealth();
    IHealthComponentProvider healthComponentProvider { get; }
    IEnemyStatProvider enemyStatProvider { get; }
    event Action EnemySpawnedEvent;
    event Action EnemyIsDeadEvent;
    EnemyTypeData enemyTypeData { get; }
    IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> currentAppliedDebuff { get; }
    void ClearDebuff();
    CircleCollider2D statusCollider { get; }
    void ApplyElementDebuff(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> debuffs, Vector2 pos = default);
    void ApplyElementDebuff(DebuffElementData debuff,Vector2 pos = default);
    void TakeDamage(float damage, bool bCritical,Vector2 pos, IReadOnlyDictionary<BulletElementType, BulletElementData> _bulletElements = null);
    void TakeCollideDamage(float damage, bool bCritical,Vector2 pos, IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffElements = null);
    void ReleaseDebuff(DebuffElementData debuffElementData);
    void ReleaseDebuff(DebuffElementEffectType type);
}
