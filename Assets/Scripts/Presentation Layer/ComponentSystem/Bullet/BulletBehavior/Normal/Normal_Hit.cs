using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/Normal_Hit")]
public class Normal_Hit : BulletBehavior_ProjectileHit
{
    public override void Enter()
    {
        base.Enter();

        var colliders = CheckExplosion();
        foreach(var collider in colliders)
        {
            ApplyDamage(collider);
            ApplyKnockBack(collider, 5f);
        }
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