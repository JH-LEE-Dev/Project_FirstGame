using UnityEngine;
[CreateAssetMenu(menuName = "Strategy/BulletBehavior/PrismBolt_Fly")]
public class PrismBolt_Fly : BulletBehavior_ProjectileFly
{

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (bBehaviorEnd)
            return;



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
