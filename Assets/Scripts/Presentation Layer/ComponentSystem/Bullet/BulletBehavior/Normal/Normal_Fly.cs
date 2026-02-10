using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/Normal_Fly")]
public class Normal_Fly : BulletBehavior_ProjectileFly
{
    public override void Enter()
    {
        base.Enter();
        speed = 1f;
    }

    protected override Vector2 ComputeNextPosition(Vector2 current)
    {
        return current + bullet.flyDir * speed * Time.deltaTime;
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