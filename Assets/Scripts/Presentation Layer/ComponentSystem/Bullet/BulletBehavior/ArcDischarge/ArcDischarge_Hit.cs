using UnityEngine;

public class ArcDischarge_Hit : ArcDischargeBehavior
{
    public override void Enter()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void End()
    {
        BulletBehaviorEndEvent?.Invoke();
    }

    public override void Exit()
    {
        BulletEffectEndEvent?.Invoke();
    }
}
