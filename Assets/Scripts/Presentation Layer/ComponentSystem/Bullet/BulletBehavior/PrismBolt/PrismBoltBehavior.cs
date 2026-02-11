using UnityEngine;

public abstract class PrismBoltBehavior : BulletBehavior_Projectile
{
    protected PrismBolt prismBolt;

    public override void Initialize(Bullet owner, ICharacterStatProvider _characterStatProvider,
                                    IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(owner, _characterStatProvider, _bulletEffectProvider, _damageSystem);
        prismBolt = owner as PrismBolt;
    }
}
