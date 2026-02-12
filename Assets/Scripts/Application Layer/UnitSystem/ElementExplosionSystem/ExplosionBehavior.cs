using UnityEngine;
using System;

public abstract class ExplosionBehavior : ScriptableObject
{
    public event Action ExplosionEndEvent;
    public event Action<Collider2D[]> ExplosionApplyRequestEvent;
    
    public ElementExplosionType elementExplosionType;

    public void ApplyExplosion(Collider2D[] colliders)
    {
        ExplosionApplyRequestEvent?.Invoke(colliders);
    }

    public abstract void Explode();
}