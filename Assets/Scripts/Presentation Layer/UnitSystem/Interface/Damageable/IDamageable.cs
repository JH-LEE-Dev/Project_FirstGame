using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, bool bCritical, IReadOnlyDictionary<BulletElementType, BulletElementData> _bulletElements = null);
    void TakeCollideDamage(float damage, bool bCritical, IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffElements = null);
    void KnockBack(Vector2 dir, float power);
    void ApplyWeakness(int turnCnt);
    void ApplyElementDebuff(DebuffElementEffectType debuffElementEffectType, int turnCnt);
}
