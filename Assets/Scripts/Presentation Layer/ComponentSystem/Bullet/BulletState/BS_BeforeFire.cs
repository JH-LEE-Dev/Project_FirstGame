using UnityEngine;

public class BS_BeforeFire : BulletState
{
    public override void BulletFireIsFinished()
    {
        ctx.bullet.BulletEffectIsFinished();
    }

    public override void CurrentStateIsEnd()
    {
        ctx.stateMachine.ChangeState<BS_Fly>();
    }

    public override void Enter()
    {
        behavior = ctx.behaviorData.behavior_BeforeFire;

        if (behavior != null)
        {
            behavior.BulletBehaviorEndEvent -= CurrentStateIsEnd;
            behavior.BulletBehaviorEndEvent += CurrentStateIsEnd;

            behavior.BulletEffectEndEvent -= BulletFireIsFinished;
            behavior.BulletEffectEndEvent += BulletFireIsFinished;

            behavior.Enter();
        }
    }

    public override void Exit()
    {
        if (behavior != null)
        {
            behavior.BulletBehaviorEndEvent -= CurrentStateIsEnd;

            behavior.BulletEffectEndEvent -= BulletFireIsFinished;
        }
    }

    public override void UpdateState()
    {
        if (behavior != null)
            behavior.Update();
    }
}
