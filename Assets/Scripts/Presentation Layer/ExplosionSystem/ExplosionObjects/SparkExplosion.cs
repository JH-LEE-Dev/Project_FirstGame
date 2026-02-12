using UnityEngine;

public class SparkExplosion : Explosion
{
    [HideInInspector] public Animator animator;

    public override void Initialize()
    {
        base.Initialize();
        explosionBehavior.Initialize(this);
        animator = GetComponentInChildren<Animator>();
    }

    public override void Explode(Vector2 pos)
    {
        explosionBehavior.Explode(pos);
    }
}
