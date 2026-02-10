using UnityEngine;

public class BS_BeforeFire : BulletState
{
    public override void CurrentStateIsEnd()
    {
        ctx.stateMachine.ChangeState<BS_Fly>();
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
