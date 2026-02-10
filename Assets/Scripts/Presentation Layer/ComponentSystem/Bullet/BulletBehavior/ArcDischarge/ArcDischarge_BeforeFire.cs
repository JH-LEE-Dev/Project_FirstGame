using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/ArcDischarge/BeforeFire")]
public class ArcDischarge_BeforeFire : ArcDischargeBehavior
{
    public override void Enter()
    {
        bUpdateEnd = false;
        Debug.Log("너 들어왔어?");
    }

    public override void Update()
    {
        if (true == bUpdateEnd)
            return;

        End();
    }

    public override void End()
    {
        bUpdateEnd = true;
        BulletBehaviorEndEvent?.Invoke();
    }

    public override void Exit()
    {
        BulletEffectEndEvent?.Invoke();
    }
}
