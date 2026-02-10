using UnityEngine;
using System;

public abstract class ExplosionBehavior : ScriptableObject
{
    public event Action ExplosionEndEvent;
    
    public ElementExplosionType elementExplosionType;

    public abstract void Explode();
}