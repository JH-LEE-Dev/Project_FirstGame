using UnityEngine;
using System;

public class ExplosionBehavior : ScriptableObject
{
    //¿Ã∫•∆Æ
    public event Action<ExplosionBehavior> ExplosionEndEvent;
    public event Action<ElementExplosionType,Collider2D[]> ExplosionApplyRequestEvent;

    public ElementExplosionType explosionType;

    public void ApplyExplosion(Collider2D[] colliders)
    {
        ExplosionApplyRequestEvent?.Invoke(explosionType,colliders);
    }

    public virtual void Explode()
    {

    }

    protected void ExplosionEnd()
    {
        ExplosionEndEvent?.Invoke(this);
    }
}