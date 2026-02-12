using UnityEngine;
using System;

public class ExplosionBehavior : ScriptableObject
{
    //¿Ã∫•∆Æ
    public event Action ExplosionEndEvent;
    public event Action<Collider2D[]> ExplosionApplyRequestEvent;

    public void ApplyExplosion(Collider2D[] colliders)
    {
        ExplosionApplyRequestEvent?.Invoke(colliders);
    }

    public virtual void Explode()
    {

    }

    protected void ExplosionEnd()
    {
        ExplosionEndEvent?.Invoke();
    }
}