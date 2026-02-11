using System.Collections.Generic;
using UnityEngine;

public class BulletStateCtx
{
    public BulletStateMachine stateMachine;
    public ICharacterStatProvider characterStatProvider;
    public IBulletEffectProvider bulletEffectProvider;
    public Bullet bullet;
    public BulletBehaviorData behaviorData;


    public void Initialize(BulletStateMachine _bulletStateMachine,ICharacterStatProvider _characterStatProvider,
        IBulletEffectProvider _bulletEffectProvider,BulletBehaviorData _data,Bullet _bullet)
    {
        stateMachine = _bulletStateMachine;
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectProvider;
        behaviorData = _data;
        bullet = _bullet;
    }
}
