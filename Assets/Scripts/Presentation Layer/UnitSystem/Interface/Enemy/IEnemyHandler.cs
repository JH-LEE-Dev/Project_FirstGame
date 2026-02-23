using UnityEngine;
using System;
using System.Collections.Generic;

public interface IEnemyHandler
{
    IEnemyData enemyData { get; }
    void ClearDebuff();
    CircleCollider2D statusCollider { get; }
    void ApplyElementDebuff(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> debuffs, Vector2 pos = default);
    void ApplyElementDebuff(DebuffElementData debuff,Vector2 pos = default);
    void TakeDamage(float damage, bool bCritical,Vector2 pos, IReadOnlyDictionary<BulletElementType, BulletElementData> _bulletElements = null);
    void TakeCollideDamage(float damage, bool bCritical,Vector2 pos, IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffElements = null);
    void ReleaseDebuff(DebuffElementData debuffElementData);
    void ReleaseDebuff(DebuffElementEffectType type);
}
