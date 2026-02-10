using UnityEngine;

public abstract class BulletBehavior_ProjectileHit : BulletBehavior
{
    public override void Enter()
    {
        base.Enter();
    }
    public override void End()
    {
        base.End();
    }
    public override void Exit()
    {
        base.Exit();
    }

    // 범위만 체크하고, Collider들을 뱉는 함수
    protected Collider2D[] CheckExplosion()
    {
        return Physics2D.OverlapCircleAll(
            bullet.transform.position,
            GetRangeRadius(),
            bullet.targetMask);
    }

    protected float GetRangeRadius()
    {
        return bullet.range + bullet.range * (characterStatProvider.attackRange * 0.01f);
    }

}
