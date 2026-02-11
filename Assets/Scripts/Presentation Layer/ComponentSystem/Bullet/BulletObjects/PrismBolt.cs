using System.Collections.Generic;
using UnityEngine;

public class PrismBolt : Bullet
{
    [SerializeField] public CircleCollider2D circleCollider;
    [SerializeField] public CircleCollider2D explosionRangeCollider;
    [SerializeField] public LayerMask targetMask;
    [SerializeField] public LayerMask outOfRangeMask;
    public SpriteRenderer sr { get; private set; }
    public Vector2 prevPosition { get; private set; }
    public float range { get; private set; }

    [SerializeField] private PrismBolt_BeforeFire behavior_BeforeFire_prefab;
    [SerializeField] private PrismBolt_Fly behavior_Fly_prefab;
    [SerializeField] private PrismBolt_Hit behavior_Hit_prefab;

    private PrismBolt_BeforeFire behavior_BeforeFire;
    private PrismBolt_Fly behavior_Fly;
    private PrismBolt_Hit behavior_Hit;

    protected override void ReadyBulletAttributes(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        behavior_BeforeFire = Instantiate(behavior_BeforeFire_prefab);
        behavior_Fly = Instantiate(behavior_Fly_prefab);
        behavior_Hit = Instantiate(behavior_Hit_prefab);

        BulletBehaviorData behaviourData = new BulletBehaviorData();
        behaviourData.behavior_BeforeFire = behavior_BeforeFire;
        behaviourData.behavior_Fly = behavior_Fly;
        behaviourData.behavior_Hit = behavior_Hit;

        stateMachine.SetBulletBehaviors(behaviourData);

        behavior_BeforeFire.Initialize(/*this,*/_characterStatProvider, _bulletEffectProvider, _damageSystem);
        behavior_Fly.Initialize(/*this,*/_characterStatProvider, _bulletEffectProvider, _damageSystem);
        behavior_Hit.Initialize(/*this,*/_characterStatProvider, _bulletEffectProvider, _damageSystem);
    }

    public override void Initialize(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(_characterStatProvider, _bulletEffectProvider,_damageSystem);

        ReadyBulletAttributes(_characterStatProvider, _bulletEffectProvider, _damageSystem);
    }
}
