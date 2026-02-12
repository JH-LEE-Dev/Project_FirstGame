using UnityEngine;
[CreateAssetMenu(menuName = "Strategy/BulletBehavior/AquaBurst/AquaBurst_BeforeFire")]

public class AquaBurst_BeforeFire : AquaBurstBehavior
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        End();
    }

    public override void End()
    {
        base.End();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
