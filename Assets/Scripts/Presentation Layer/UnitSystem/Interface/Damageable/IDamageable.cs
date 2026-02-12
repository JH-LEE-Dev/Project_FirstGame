using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, bool bCritical,Vector2 pos, IReadOnlyDictionary<BulletElementType, BulletElementData> _bulletElements = null);
    void TakeCollideDamage(float damage, bool bCritical, Vector2 pos,IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffElements = null);
    void KnockBack(Vector2 dir, float power);
    void ApplyWeakness(int turnCnt);
    void ApplyElementDebuff(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> debuffs);
    void ApplyElementDebuff(DebuffElementData debuff);
    Transform GetTransform();
}
