using UnityEngine;
[CreateAssetMenu(menuName = "Strategy/BulletBehavior/PrismBolt_Hit")]
public class PrismBolt_Hit : BulletBehavior_ProjectileHit
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
