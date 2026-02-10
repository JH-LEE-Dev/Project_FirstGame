using UnityEngine;
using System;

public abstract class BulletBehavior : ScriptableObject
{
    //총알의 단계별 행동이 끝났을 때 호출.
    public Action BulletBehaviorEndEvent;

    //외부 의존성
    protected ICharacterStatProvider characterStatProvider;
    protected IBulletEffectProvider bulletEffectProvider;
    protected Bullet bullet;

    //속성 및 변수.
    [SerializeField] protected float speed = 1f;
    [SerializeField] protected float knockBackPower = 1f;
    protected Collider2D directHitObject;
    protected Vector2 prevPosition;
    protected float baseDamage = 0f;
    protected float elemExplosionDamage = 0f;
    protected bool bBehaviorEnd = false;

    public void Initialize(Bullet _bullet, ICharacterStatProvider _characterStatProvider,
    IBulletEffectProvider _bulletEffectProvider)
    {
        bullet = _bullet;
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectProvider;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void End();
}