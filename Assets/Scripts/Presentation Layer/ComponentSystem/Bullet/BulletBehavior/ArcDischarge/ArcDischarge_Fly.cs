using UnityEngine;

public class ArcDischarge_Fly : ArcDischargeBehavior
{
    public override void Enter()
    {
        bUpdateEnd = false;
    }

    public override void Update()
    {
        if (true == bUpdateEnd)
            return;


    }

    public override void End()
    {
        bUpdateEnd = true;
        BulletBehaviorEndEvent?.Invoke();
    }
}
