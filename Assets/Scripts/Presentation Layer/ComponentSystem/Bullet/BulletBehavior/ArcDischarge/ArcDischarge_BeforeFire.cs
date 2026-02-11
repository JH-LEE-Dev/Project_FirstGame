using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/ArcDischarge/BeforeFire")]
public class ArcDischarge_BeforeFire : ArcDischargeBehavior
{
    public override void Enter()
    {
        bBehaviorEnd = false;
        Debug.Log("1");
    }

    public override void Update()
    {
        if (true == bBehaviorEnd)
            return;

        End();
    }

    public override void End()
    {
        bBehaviorEnd = true;
        BulletBehaviorEndEvent?.Invoke();
    }

    public override void Exit()
    {
        BulletEffectEndEvent?.Invoke();
    }
}
