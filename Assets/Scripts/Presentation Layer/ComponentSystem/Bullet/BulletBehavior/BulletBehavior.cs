using System;
using System.Collections.Generic;
using UnityEngine;

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
    protected Bullet bullet;


    protected static readonly Collider2D[] overlapBuffer = new Collider2D[64];
    protected static readonly Collider2D[] sectorResultBuffer = new Collider2D[64];
    protected static ContactFilter2D filter;
    /// <summary>
    /// 시스템 속성 존 --------------------------------------
    /// </summary>
    protected List<IDamageable> damagedObjects = new List<IDamageable>(SYSTEM_VAR.maxEnemyCount);
    protected bool bBehaviorEnd = false;

    public virtual void Initialize(Bullet owner, ICharacterStatProvider _characterStatProvider,
    IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        bullet = owner;
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectProvider;
        damageSystem = _damageSystem;

        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = bullet.targetMask;
        filter.useTriggers = true;
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
        damagedObjects.Clear();
        bBehaviorEnd = true;
        BulletEffectEndEvent?.Invoke();
    }

    // 콜라이더 주인장한테 데미지 및 상태이상을 주는 함수
    protected virtual void ApplyDamage(Collider2D other, float damage, bool bCritical, Vector2 pos)
    {
        // 데미지 처리

        IDamageable hit = other.GetComponent<IDamageable>();

        if (hit != null)
        {
            if (damagedObjects.Contains(hit) == false)
            {
                damagedObjects.Add(hit);

                hit.ApplyElementDebuff(bulletEffectProvider.currentDebuffElementTypes);
                hit.ApplyWeakness(characterStatProvider.weaknessTurnCnt);
            }

            hit.TakeDamage(damage, bCritical, pos, bulletEffectProvider.currentEffectElements);
        }
    }

    protected virtual void ApplyAdditionalDamage(Collider2D other, AdditionalAttackData _data, Vector2 pos )
    {
        // 데미지 처리
        IDamageable hit = other.GetComponent<IDamageable>();

        if (hit != null)
        {
            if (damagedObjects.Contains(hit) == false)
            {
                damagedObjects.Add(hit);

                hit.ApplyElementDebuff(_data.debuffData);
                hit.ApplyWeakness(characterStatProvider.weaknessTurnCnt);
            }

            hit.TakeDamage(_data.resultDamage, _data.bCritical, pos);
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

    protected int CheckSector(Vector2 pos, float range, float angleDeg, Vector2 forwardDir)
    {
        float r = UpscaleRange(range);

        int hitCount = Physics2D.OverlapCircle(pos, r, filter, overlapBuffer);
        if (hitCount == 0) return 0;

        if (forwardDir.sqrMagnitude < 0.0001f)
            forwardDir = Vector2.up;
        forwardDir.Normalize();

        float half = angleDeg * 0.5f;
        float cosThreshold = Mathf.Cos(half * Mathf.Deg2Rad);

        int resultCount = 0;

        for (int i = 0; i < hitCount; i++)
        {
            var col = overlapBuffer[i];
            if (!col) continue;

            Vector2 to = (Vector2)col.bounds.ClosestPoint(pos) - pos;
            float sqr = to.sqrMagnitude;

            if (sqr <= 0.000001f)
            {
                sectorResultBuffer[resultCount++] = col;
                continue;
            }

            to /= Mathf.Sqrt(sqr); // normalize
            float dot = Vector2.Dot(forwardDir, to);

            if (dot >= cosThreshold)
            {
                sectorResultBuffer[resultCount++] = col;
                if (resultCount >= sectorResultBuffer.Length) break;
            }
        }

        return resultCount;
    }

    protected float UpscaleRange(float range)
    {
        return range + range * (characterStatProvider.attackRange * 0.01f);
    }
}