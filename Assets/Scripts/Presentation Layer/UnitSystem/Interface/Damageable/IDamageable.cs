using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage,bool bCritical);

    void KnockBack(Vector2 dir,float power);

    void ApplyWeakness(int turnCnt);
    void ApplyElementDebuff(DebuffElementEffectType debuffElementEffectType,int turnCnt);
}
