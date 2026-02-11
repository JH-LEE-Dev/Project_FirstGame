using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/PrismBolt_BeforeFire")]
public class PrismBolt_BeforeFire : PrismBoltBehavior
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (bBehaviorEnd)
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
