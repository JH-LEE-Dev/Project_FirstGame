using UnityEngine;

public class BulletStateCtx
{
    public BulletStateMachine stateMachine;
    public ICharacterStatProvider characterStatProvider;
    public IBulletEffectProvider bulletEffectProvider;
    public Bullet bullet;

    private DamageCalcComponent damageCalcComponent;

    public void Initialize(BulletStateMachine _bulletStateMachine,ICharacterStatProvider _characterStatProvider,
        IBulletEffectProvider _bulletEffectProvider,Bullet _bullet,DamageCalcComponent _damageCalcComponent)
    {
        stateMachine = _bulletStateMachine;
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectProvider;
        bullet = _bullet;
        damageCalcComponent = _damageCalcComponent;
    }

    public T GetDamageCalc<T>() where T : class
    {
        return damageCalcComponent as T;
    }
}
