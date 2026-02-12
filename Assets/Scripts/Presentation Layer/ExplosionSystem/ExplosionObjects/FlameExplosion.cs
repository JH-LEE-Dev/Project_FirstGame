using UnityEngine;

public class FlameExplosion : Explosion
{
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Explode(Vector2 pos)
    {
        explosionBehavior.Explode();
    }
}
