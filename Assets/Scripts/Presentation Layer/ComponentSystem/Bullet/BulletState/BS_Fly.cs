using UnityEngine;

public class BS_Fly : BulletState
{
    public override void CurrentStateIsEnd()
    {
        ctx.stateMachine.ChangeState<BS_Hit>();
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
