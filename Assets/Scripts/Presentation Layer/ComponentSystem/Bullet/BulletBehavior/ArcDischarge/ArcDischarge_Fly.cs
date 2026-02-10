using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/ArcDischarge_Fly")]
public class ArcDischarge_Fly : ArcDischargeBehavior
{
    public override void Enter()
    {
        bUpdateEnd = false;
        firstTarget = null;
    }

    public override void Update()
    {
        if (true == bUpdateEnd)
            return;

        
    }

    public override void End()
    {
        bUpdateEnd = true;
        BulletBehaviorEndEvent?.Invoke();
    }

    public override void Exit()
    {
        bUpdateEnd = true;
        BulletEffectEndEvent?.Invoke();
    }


    private bool CheckCollision_Enemy() //총알에 직격한 적이 있는지 체크.
    {
        Vector2 bulletStartPos = bullet.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            bulletStartPos,
            bullet.flyDir,
            Mathf.Infinity,
            bullet.targetMask
        );

        firstTarget = hit.collider;

        if (null != firstTarget)
        {
            End();
            return true;
        }

        return false;
    }

    private bool CheckCollision_OutofRange()
    {
        Vector2 bulletStartPos = bullet.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            bulletStartPos,
            bullet.flyDir,
            Mathf.Infinity,
            bullet.outOfRangeMask
        );

        if (null != hit.collider)
        {
            Exit();
            return true;
        }

        return false;
    }
}
