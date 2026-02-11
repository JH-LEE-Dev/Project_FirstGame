using UnityEngine;
[CreateAssetMenu(menuName = "Strategy/BulletBehavior/PrismBolt/PrismBolt_Hit")]
public class PrismBolt_Hit : PrismBoltBehavior
{
    public override void Enter()
    {
        base.Enter();

        var enemys = CheckExplosion();
        foreach (var enemy in enemys)
        {
            ApplyDamage(enemy);
            ApplyKnockBack(enemy);
        }
    }


    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        Exit();
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
