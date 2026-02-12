using UnityEngine;

public abstract class AquaBurstBehavior : BulletBehavior_Projectile
{
    protected AquaBurst aquaBurst;

    public override void Initialize(Bullet owner, ICharacterStatProvider _characterStatProvider,
                                    IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(owner, _characterStatProvider, _bulletEffectProvider, _damageSystem);
        aquaBurst = owner as AquaBurst;
    }
}
