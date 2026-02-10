using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/Normal_BeforeFire")]
public class Normal_BeforeFire : BulletBehavior_ProjectileBeforeFire
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