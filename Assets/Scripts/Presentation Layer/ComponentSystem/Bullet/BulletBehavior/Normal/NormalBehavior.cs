using JetBrains.Annotations;
using UnityEngine;

public class NormalBehavior : BulletBehavior
{
    //속성 및 변수.
    [SerializeField] protected float speed = 1f;
    [SerializeField] protected float knockBackPower = 1f;
    protected Collider2D directHitObject;
    protected Vector2 prevPosition;
    protected float baseDamage = 0f;
    protected float elemExplosionDamage = 0f;

    public override void End()
    {

    }

    public override void Exit()
    {
        BulletEffectEndEvent?.Invoke();
    }

    public override void Enter()
    {

    }

    public override void Update()
    {

    }
}
