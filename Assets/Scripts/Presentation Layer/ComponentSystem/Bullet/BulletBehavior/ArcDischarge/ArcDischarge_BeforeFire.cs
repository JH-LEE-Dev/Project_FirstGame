using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/ArcDischarge/BeforeFire")]
public class ArcDischarge_BeforeFire : ArcDischargeBehavior
{
    public override void Enter()
    {
        bBehaviorEnd = false;

        arcDischarge.AllDeActivateVFX();
    }

    public override void Update()
    {
        if (true == bBehaviorEnd)
            return;

        End();
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
