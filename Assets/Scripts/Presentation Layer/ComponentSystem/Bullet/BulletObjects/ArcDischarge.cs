using UnityEngine;

public class ArcDischarge : Bullet
{
    public SpriteRenderer sr { get; private set; }

    public override void Initialize(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(_characterStatProvider, _bulletEffectProvider, _damageSystem);
    }
}
