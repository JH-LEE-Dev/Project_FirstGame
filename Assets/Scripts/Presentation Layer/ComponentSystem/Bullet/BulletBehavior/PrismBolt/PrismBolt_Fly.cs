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
            prismBolt.animator.gameObject.SetActive(true);
            prismBolt.animator.enabled = true;
            prismBolt.animator.Play(0, 0, 0f);
            prismBolt.animator.speed = 1f;    
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
            prismBolt.animator.gameObject.SetActive(false);
            prismBolt.animator.enabled = false;
        }
        base.End();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
