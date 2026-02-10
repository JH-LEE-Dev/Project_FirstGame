using UnityEngine;

public abstract class BulletState : MonoBehaviour
{
    [SerializeField] protected BulletBehavior bulletBehavior;

    protected BulletStateCtx ctx;

    public void Initialize(BulletStateCtx _ctx)
    {
        ctx = _ctx;

        bulletBehavior.Initialize(ctx.bullet, ctx.characterStatProvider, ctx.bulletEffectProvider);

        bulletBehavior.BulletBehaviorEndEvent -= CurrentStateIsEnd;
        bulletBehavior.BulletBehaviorEndEvent += CurrentStateIsEnd;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void UpdateState();
    public abstract void CurrentStateIsEnd();
}