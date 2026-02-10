using JetBrains.Annotations;
using UnityEngine;

public class NormalBehavior : BulletBehavior
{
    public override void End()
    {

    }

    public override void Exit()
    {
        BulletEffectEndEvent?.Invoke();
    }

    public override void Enter()
    {

    }

    public override void Update()
    {

    }
}
