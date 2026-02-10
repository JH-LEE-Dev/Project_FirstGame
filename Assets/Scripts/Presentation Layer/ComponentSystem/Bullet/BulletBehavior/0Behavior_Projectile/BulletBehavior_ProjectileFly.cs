using UnityEngine;

public abstract class BulletBehavior_ProjectileFly : BulletBehavior
{
    protected float speed;
    protected Vector2 prevPosition;

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

    // 총알에 직격한 적이 있는지 체크.
    protected virtual Collider2D CheckCollision_Enemy(Vector2 delta, float distance)
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            prevPosition,
            bullet.range,
            delta.normalized,
            distance,
            bullet.targetMask
        );

        if (hit.collider != null)
        {
            return hit.collider;
        }
        return null;
    }
    // 총알이 범위를 벗어났는지 체크.
    protected virtual bool CheckCollision_OutofRange(Vector2 delta, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            prevPosition,
            delta.normalized,
            distance,
            bullet.outOfRangeMask
        );

        if (hit.collider != null)
            return true;

        return false;
    }
}
