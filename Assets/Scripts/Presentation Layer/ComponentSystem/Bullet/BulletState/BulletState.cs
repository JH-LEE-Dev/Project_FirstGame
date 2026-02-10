using UnityEngine;
using System.Collections.Generic;

public abstract class BulletState
{
    protected BulletStateCtx ctx;
    protected BulletBehavior behavior;

    public void Initialize(BulletStateCtx _ctx)
    {
        ctx = _ctx;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void UpdateState();
    public abstract void CurrentStateIsEnd();
    public abstract void BulletFireIsFinished();
}