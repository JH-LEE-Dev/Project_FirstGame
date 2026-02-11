using UnityEngine;
[CreateAssetMenu(menuName = "Strategy/BulletBehavior/PrismBolt/PrismBolt_Fly")]
public class PrismBolt_Fly : PrismBoltBehavior
{

    public override void Enter()
    {
        base.Enter();
        SetBulletInitialPosition();

        if (prismBolt.animator != null)
        {
            prismBolt.animator.enabled = true;     // 혹시 꺼져있던 경우 대비
            prismBolt.animator.Play(0, 0, 0f);     // 0번 state를 처음부터
            prismBolt.animator.speed = 1f;         // 필요하면 가속/감속 가능
        }
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
        if (prismBolt.animator != null)
        {
            prismBolt.animator.enabled = false; // 완전 정지
        }
        base.End();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
