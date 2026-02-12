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
            prismBolt.animator.Update(0f);
        }
        float fixScale = UpscaleRange(1f);
        prismBolt.transform.localScale = new Vector2(fixScale, fixScale);
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
        StopAnim();
        base.End();
    }

    public override void Exit()
    {
        StopAnim();
        base.Exit();
    }

    private void StopAnim()
    {
        if (prismBolt.animator != null)
        {
            prismBolt.animator.enabled = false;
            prismBolt.animator.gameObject.SetActive(false);
        }
    }
}
