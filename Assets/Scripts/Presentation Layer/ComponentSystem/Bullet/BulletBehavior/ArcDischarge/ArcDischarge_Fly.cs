using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/ArcDischarge/Fly")]
public class ArcDischarge_Fly : ArcDischargeBehavior
{
    public override void Enter()
    {
        bBehaviorEnd = false;
        arcDischarge.firstTarget = null;
        Debug.Log("2");
    }

    public override void Update()
    {
        if (true == bBehaviorEnd)
            return;

        if (true == CheckCollision_Enemy())
        {
            End();
            return;
        }

        if (true == CheckCollision_OutofRange())
        {
            Exit();
            return;
        }
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
        Vector2 bulletStartPos = bullet.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            bullet.initPosition,
            bullet.initDir,
            Mathf.Infinity,
            bullet.targetMask
        );

        arcDischarge.firstTarget = hit.collider;

        if (null != arcDischarge.firstTarget) 
            return true;

        return false;
    }

    private bool CheckCollision_OutofRange()
    {
        Vector2 bulletStartPos = bullet.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            bullet.initPosition,
            bullet.initDir,
            Mathf.Infinity,
            bullet.outOfRangeMask
        );

        if (null != hit.collider)
            return true;

        return false;
    }
}
