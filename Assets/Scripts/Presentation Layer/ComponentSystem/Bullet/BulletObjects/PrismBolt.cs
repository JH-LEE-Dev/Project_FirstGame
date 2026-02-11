using System.Collections.Generic;
using UnityEngine;

public class PrismBolt : Bullet
{
    public SpriteRenderer sr { get; private set; }

    public float speed;

    public override void Initialize(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(_characterStatProvider, _bulletEffectProvider, _damageSystem);

        originRange = 0.05f;
        originExplosionRange = 1f;
    }
}
