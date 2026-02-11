using Mono.Cecil;
using UnityEngine;

public abstract class BulletBehavior_ProjectileFly : BulletBehavior
{
    protected float speed;
    protected Vector2 prevPosition;

    public override void Enter()
    {
        base.Enter();
        prevPosition = bullet.projectileObj.prevPosition;
    }

    public sealed override void Update()
    {
        if (bBehaviorEnd) 
            return;

        Vector2 current = (Vector2)bullet.projectileObj.transform.position;
        Vector2 next = ComputeNextPosition(current);

        Vector2 delta = next - prevPosition;
        float distance = delta.magnitude;

        if (distance < 0.00001f)
        {
            bullet.projectileObj.transform.position = next;
            prevPosition = next;
            return;
        }

        Vector2 dir = delta / distance;

        if (CheckCollision_Enemy(dir, distance, out var hit) != null)
        {
            Vector2 impactPoint = hit.point;
            bullet.projectileObj.transform.position = impactPoint;
            End();
            return;
        }

        if (CheckCollision_OutofRange(dir, distance))
        {
            Exit();
            return;
        }

        bullet.projectileObj.transform.position = next;
        prevPosition = next;
    }

    protected abstract Vector2 ComputeNextPosition(Vector2 currentPosition);



    // 날아가다가 직격한 적이 있는지 체크.
    protected virtual Collider2D CheckCollision_Enemy(Vector2 dir, float distance, out RaycastHit2D hit)
    {
        hit = Physics2D.CircleCast(
            prevPosition,
            bullet.projectileObj.range,
            dir,
            distance,
            bullet.projectileObj.targetMask
        );

        if (hit.collider != null)
        {
            return hit.collider;
        }

        return null;
    }
    // 날아가다가 범위를 벗어났는지 체크.
    protected virtual bool CheckCollision_OutofRange(Vector2 delta, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            prevPosition,
            delta.normalized,
            distance,
            bullet.projectileObj.outOfRangeMask
        );

        if (hit.collider != null)
            return true;

        return false;
    }

    public override void End()
    {
        base.End();
    }
    public override void Exit()
    {
        base.Exit();
    }

}
