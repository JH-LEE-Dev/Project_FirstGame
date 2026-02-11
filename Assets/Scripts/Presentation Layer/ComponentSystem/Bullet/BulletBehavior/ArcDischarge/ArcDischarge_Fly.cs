using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/ArcDischarge/Fly")]
public class ArcDischarge_Fly : ArcDischargeBehavior
{
    public override void Enter()
    {
        bBehaviorEnd = false;
        firstTarget = null;
    }

    public override void Update()
    {
        if (true == bBehaviorEnd)
            return;

        if (true == CheckCollision_Enemy())
            return;

        if (true == CheckCollision_OutofRange())
            return;
    }

    public override void End()
    {
        bBehaviorEnd = true;
        BulletBehaviorEndEvent?.Invoke();
    }

    public override void Exit()
    {
        bBehaviorEnd = true;
        BulletEffectEndEvent?.Invoke();
    }


    private bool CheckCollision_Enemy() //총알에 직격한 적이 있는지 체크.
    {
        //Vector2 bulletStartPos = bullet.transform.position;

        RaycastHit2D hit = default;
        //RaycastHit2D hit = Physics2D.Raycast(
        //    bulletStartPos,
        //    bullet.flyDir,
        //    Mathf.Infinity,
        //    bullet.nonProjectileObj.targetMask
        //);

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
        //Vector2 bulletStartPos = bullet.transform.position;

        RaycastHit2D hit = default;
        //RaycastHit2D hit = Physics2D.Raycast(
        //    bulletStartPos,
        //    bullet.flyDir,
        //    Mathf.Infinity,
        //    bullet.nonProjectileObj.outOfRangeMask
        //);

        if (null != hit.collider)
        {
            Exit();
            return true;
        }

        return false;
    }
}
