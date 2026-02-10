using System.Collections.Generic;
using UnityEngine;

public class BulletStateCtx
{
    public BulletStateMachine stateMachine;
    public ICharacterStatProvider characterStatProvider;
    public IBulletEffectProvider bulletEffectProvider;
    public IReadOnlyDictionary<BulletType, BulletBehaviorData> bulletBehaviors;
    public Bullet bullet;

    private DamageCalcComponent damageCalcComponent;


    public void Initialize(BulletStateMachine _bulletStateMachine,ICharacterStatProvider _characterStatProvider,
        IBulletEffectProvider _bulletEffectProvider,Bullet _bullet,DamageCalcComponent _damageCalcComponent,
        IReadOnlyDictionary<BulletType, BulletBehaviorData> _bulletBehaviors)
    {
        stateMachine = _bulletStateMachine;
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectProvider;
        bullet = _bullet;
        damageCalcComponent = _damageCalcComponent;
        bulletBehaviors = _bulletBehaviors;
    }
}
