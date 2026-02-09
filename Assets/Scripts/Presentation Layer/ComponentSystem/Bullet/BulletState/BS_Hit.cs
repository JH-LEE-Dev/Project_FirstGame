using UnityEngine;

public class BS_Hit : BulletState
{
    public override void CurrentStateIsEnd()
    {
        ctx.bullet.BulletEffectIsFinished();
    }

    public override void Enter()
    {
        bulletBehavior.Enter();
    }

    public override void Exit()
    {

    }

    public override void UpdateState()
    {
        bulletBehavior.Update();
    }
}
