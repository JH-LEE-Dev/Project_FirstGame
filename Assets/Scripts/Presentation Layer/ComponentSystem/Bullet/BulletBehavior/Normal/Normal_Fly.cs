using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/Normal_Fly")]
public class Normal_Fly : BulletBehavior_ProjectileFly
{
    public override void Enter()
    {
        base.Enter();

        speed = 1f;
        prevPosition = bullet.prevPosition;
    }

    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        Vector2 currentPosition = (Vector2)bullet.transform.position + bullet.flyDir * speed * Time.deltaTime;
        bullet.transform.position = currentPosition;
        Vector2 delta = currentPosition - prevPosition;
        float distance = delta.magnitude;

        if (CheckCollision_Enemy(delta, distance) != null)
        {
            End();
            return;
        }

        if (CheckCollision_OutofRange(delta, distance))
        {
            Exit();
            return;
        }

        bullet.transform.position = currentPosition;
        prevPosition = currentPosition;
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