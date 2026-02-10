using UnityEngine;

public class BulletStateCtx
{
    public BulletStateMachine stateMachine;
    public ICharacterStatProvider characterStatProvider;
    public IBulletEffectProvider bulletEffectProvider;
    public Bullet bullet;

    public void Initialize(BulletStateMachine _bulletStateMachine,ICharacterStatProvider _characterStatProvider,
        IBulletEffectProvider _bulletEffectProvider,Bullet _bullet)
    {
        stateMachine = _bulletStateMachine;
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectProvider;
        bullet = _bullet;
    }
}
