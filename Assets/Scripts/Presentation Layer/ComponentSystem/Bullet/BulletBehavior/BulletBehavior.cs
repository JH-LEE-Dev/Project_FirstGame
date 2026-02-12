using UnityEngine;
using System;

public abstract class BulletBehavior : ScriptableObject
{
    //총알의 단계별 행동이 끝났을 때 호출.
    public Action BulletBehaviorEndEvent;
    //총알의 행동을 아예 종료할 때 호출.
    public Action BulletEffectEndEvent;

    //외부 의존성
    protected ICharacterStatProvider characterStatProvider;
    protected IBulletEffectProvider bulletEffectProvider;
    protected IDamageSystem damageSystem;
    protected bool bBehaviorEnd = false;

    protected Bullet bullet;

    public virtual void Initialize(Bullet owner, ICharacterStatProvider _characterStatProvider,
    IBulletEffectProvider _bulletEffectProvider,IDamageSystem _damageSystem)
    {
        bullet = owner;
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectProvider;
        damageSystem = _damageSystem;
    }

    public virtual void Enter()
    {
        bBehaviorEnd = false;
    }


    public abstract void Update();


    public virtual void End()
    {
        bBehaviorEnd = true;
        BulletBehaviorEndEvent?.Invoke();
    }


    public virtual void Exit()
    {
        bBehaviorEnd = true;
        BulletEffectEndEvent?.Invoke();
    }

    // 콜라이더 주인장한테 데미지 및 상태이상을 주는 함수
    protected virtual void ApplyDamage(Collider2D other, float damage, bool bCritical)
    {
        // 데미지 처리

        IDamageable hit = other.GetComponent<IDamageable>();

        if (hit != null)
        {
            hit.TakeDamage(damage, bCritical);
            hit.ApplyWeakness(characterStatProvider.weaknessTurnCnt);
        }
    }

    // 콜라이더 주인장한테, 원하는 넉백을 주는 함수 (충돌 지점 기준..)
    protected virtual void ApplyKnockBack(Collider2D other, float knockBackPower = 0f)
    {
        IDamageable enemy = other.GetComponent<IDamageable>();

        Vector2 dir = (Vector2)other.transform.position - (Vector2)bullet.transform.position;
        enemy.KnockBack(dir.normalized, knockBackPower);
    }

    // 범위만 체크하고, Collider들을 뱉는 함수
    protected Collider2D[] CheckRange(Vector3 pos, float range)
    {
        return Physics2D.OverlapCircleAll(
            pos,
            UpscaleRange(range),
            bullet.targetMask);
    }

    protected float UpscaleRange(float range)
    {
        return range + range * (characterStatProvider.attackRange * 0.01f);
    }
}