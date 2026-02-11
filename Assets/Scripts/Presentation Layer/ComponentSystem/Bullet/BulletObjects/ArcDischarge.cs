using UnityEngine;

public class ArcDischarge : Bullet
{
    public SpriteRenderer sr { get; private set; }

    public Collider2D firstTarget { get; set; }

    [field: SerializeField] public int maxTransference { get; set; } = 2;
    [field: SerializeField] public float finderRadius { get; set; } = 20f;
    [field: SerializeField] public float chainDelay { get; set; } = 0.1f;

    public override void Initialize(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(_characterStatProvider, _bulletEffectProvider, _damageSystem);
    }
}
