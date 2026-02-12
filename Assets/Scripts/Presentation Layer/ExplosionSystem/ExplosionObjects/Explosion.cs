using System;
using UnityEngine;

public abstract class Explosion : MonoBehaviour
{
    public event Action<Explosion> ExplosionEndEvent;
    public event Action<ElementExplosionType, Collider2D[]> ApplyExplosionEvent;

    [SerializeField] private ExplosionBehavior explosionBehavior_Prefab;
    protected ExplosionBehavior explosionBehavior;
    public ElementExplosionType elementExplosionType;

    [SerializeField] public LayerMask targetMask;
    [SerializeField] public LayerMask EarthMask;

    public virtual void Initialize()
    {
        explosionBehavior = Instantiate(explosionBehavior_Prefab);

        explosionBehavior.ExplosionEndEvent -= ExplosionEnd;
        explosionBehavior.ExplosionEndEvent += ExplosionEnd;

        explosionBehavior.ExplosionApplyRequestEvent -= ApplyExplosion;
        explosionBehavior.ExplosionApplyRequestEvent += ApplyExplosion;
    }

    private void ExplosionEnd()
    {
        ExplosionEndEvent?.Invoke(this);
    }

    private void ApplyExplosion(Collider2D[] _colliders)
    {
        ApplyExplosionEvent?.Invoke(elementExplosionType, _colliders);
    }

    public abstract void Explode(Vector2 pos);
}
