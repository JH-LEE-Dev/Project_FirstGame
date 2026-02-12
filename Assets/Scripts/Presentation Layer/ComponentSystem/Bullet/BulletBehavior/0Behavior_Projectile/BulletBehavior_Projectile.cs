using UnityEngine;

public abstract class BulletBehavior_Projectile : BulletBehavior
{
    protected Vector2 prevPosition;
    protected Collider2D directHitEnemy;


    protected enum ProjectileState
    {
        None,
        End, // Hit로 넘어감.
        Exit, // 폭발 안하고 종료.
    }

    public override void Enter()
    {
        base.Enter();
    }

    // 시작 위치로 초기화 해준다.
    protected virtual void SetBulletInitialPosition()
    {
        prevPosition = bullet.transform.position = bullet.initPosition;
    }

    // 불릿을 앞으로 나아가게 하는 함수.
    protected virtual void BulletUpdate()
    {
        ProjectileState currState = TryStop();

        if (currState == ProjectileState.End)
        {
            End();
            return;
        }
        else if (currState == ProjectileState.Exit)
        {
            Exit();
            return;
        }

        Vector2 current = (Vector2)bullet.transform.position;
        Vector2 next = ComputeNextPosition(current);

        Vector2 delta = next - prevPosition;
        float distance = delta.magnitude;

        if (distance < 0.00001f)
        {
            bullet.transform.position = next;
            prevPosition = next;
            return;
        }

        Vector2 dir = delta / distance;

        Collider2D hitCol = CheckCollision_Enemy(dir, distance, out var hit);
        if (hitCol != null)
        {
            directHitEnemy = hitCol;
            Vector2 impactPoint = hit.point;
            bullet.transform.position = impactPoint;
            End();
            return;
        }

        if (CheckCollision_OutofRange(dir, distance))
        {
            Exit();
            return;
        }

        bullet.transform.position = next;
        prevPosition = next;
    }

    // 이동 델타량 정의.
    protected virtual Vector2 ComputeNextPosition(Vector2 currentPosition)
    {
        return Vector2.zero;
    }

    // 특수한 경우 멈추게 할 수 있음.
    protected virtual ProjectileState TryStop()
    {
        return ProjectileState.None;
    }

    // 적 감지.
    protected virtual Collider2D CheckCollision_Enemy(Vector2 dir, float distance, out RaycastHit2D hit)
    {
        hit = Physics2D.CircleCast(
            prevPosition,
            UpscaleRange(bullet.originRange),
            dir,
            distance,
            bullet.targetMask
        );

        if (hit.collider != null)
        {
            return hit.collider;
        }

        return null;
    }

    // 밖으로 나갔는지.
    protected virtual bool CheckCollision_OutofRange(Vector2 dir, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            prevPosition,
            dir,
            distance,
            bullet.outOfRangeMask
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
