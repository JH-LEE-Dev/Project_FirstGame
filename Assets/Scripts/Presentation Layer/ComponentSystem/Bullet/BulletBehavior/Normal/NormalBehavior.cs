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
    protected bool bBehaviorEnd = false;

    public override void End()
    {

    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}
