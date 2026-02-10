using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/Normal_BeforeFire")]
public class Normal_BeforeFire : BulletBehavior
{
    public override void Enter()
    {
        bBehaviorEnd = false;
    }

    public override void Update()
    {
        if (bBehaviorEnd)
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