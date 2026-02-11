using UnityEngine;
[CreateAssetMenu(menuName = "Strategy/BulletBehavior/PrismBolt_Fly")]
public class PrismBolt_Fly : PrismBoltBehavior
{

    public override void Enter()
    {
        base.Enter();
        SetBulletInitialPosition();
        prismBolt.speed = 1f;
    }

    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        BulletUpdate();
    }

    protected override Vector2 ComputeNextPosition(Vector2 current)
    {
        return current + prismBolt.initDir * prismBolt.speed * Time.deltaTime;
    }

    protected override ProjectileState TryStop()
    {
        return ProjectileState.None;
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
