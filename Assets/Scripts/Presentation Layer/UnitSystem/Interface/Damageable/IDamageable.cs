using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);

    void KnockBack(Vector2 dir,float power);

    void ApplyWeakness(int turnCnt);
}
