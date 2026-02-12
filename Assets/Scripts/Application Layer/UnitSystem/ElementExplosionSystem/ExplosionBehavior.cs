using UnityEngine;
using System;

public class ExplosionBehavior : ScriptableObject
{
    //¿Ã∫•∆Æ
    public event Action ExplosionEndEvent;
    public event Action<Collider2D[]> ExplosionApplyRequestEvent;

    protected Explosion explosion;

    public virtual void Initialize(Explosion explosion)
    {
        this.explosion = explosion;
    }


    public void ApplyExplosion(Collider2D[] colliders)
    {
        ExplosionApplyRequestEvent?.Invoke(colliders);
    }

    public virtual void Explode(Vector2 pos)
    {

    }

    protected void ExplosionEnd()
    {
        ExplosionEndEvent?.Invoke();
    }
}