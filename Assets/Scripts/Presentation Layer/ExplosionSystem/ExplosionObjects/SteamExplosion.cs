using UnityEngine;

public class SteamExplosion : Explosion
{
    public override void Initialize()
    {
        base.Initialize();

    }

    public override void Explode()
    {
        explosionBehavior.Explode();
    }
}
