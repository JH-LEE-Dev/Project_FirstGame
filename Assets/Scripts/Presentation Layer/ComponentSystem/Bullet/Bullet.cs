using Mono.Cecil.Cil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public event Action BulletEffectIsFinishedEvent;

    //외부 의존성
    ICharacterStatProvider characterStatProvider;

    //statComponent로 기능 분리할 것.
    [SerializeField] float speed = 1f;
    [SerializeField] float knockBackPower = 1f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask outOfRangeMask;

    private EffectComponent effectComponent;

    private SpriteRenderer sr;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private CircleCollider2D explosionRangeCollider;

    private Vector2 flyDir;
    private Vector2 prevPosition;

    private bool bFired = false;

    private Collider2D directHitObject;

    private float range = 0f;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        effectComponent = GetComponentInChildren<EffectComponent>();

        circleCollider.enabled = false;
        explosionRangeCollider.enabled = false;
        range = explosionRangeCollider.radius;
    }

    public void Initialize(ICharacterStatProvider _characterStatProvider)
    {
        characterStatProvider = _characterStatProvider;
    }

    private void OnDestroy()
    {
        BulletEffectIsFinishedEvent = null;
    }

    private void Update()
    {
        Fly();
    }

    private void Fly()
    {
        if (bFired == false)
            return;

        Vector2 currentPosition = (Vector2)transform.position + flyDir * speed * Time.deltaTime;
        transform.position = currentPosition;
        Vector2 delta = currentPosition - prevPosition;
        float distance = delta.magnitude;

        if (CheckCollision_Enemy(delta, distance) == true)
            return;

        if (CheckCollision_OutofRange(delta, distance) == true)
            return;

        transform.position = currentPosition;
        prevPosition = currentPosition;
    }

    private bool CheckCollision_Enemy(Vector2 delta, float distance)
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            prevPosition,
            circleCollider.radius,
            delta.normalized,
            distance,
            targetMask
        );

        if (hit.collider != null)
        {
            ApplyDamage(hit.collider);
            bFired = false;

            Sound.Play("Impact", transform.position);
            DeActivateBullet();

            CheckExplosion();

            BulletEffectIsFinished();

            return true;
        }

        return false;
    }

    private void CheckExplosion()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position,
            range + range * (characterStatProvider.attackRange * 0.01f), targetMask);

        foreach (var enemy in hitEnemies)
        {
            if (enemy == directHitObject)
                continue;

            ApplyDamage(enemy);
        }
    }

    private bool CheckCollision_OutofRange(Vector2 delta, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            prevPosition,
            delta.normalized,
            distance,
            outOfRangeMask
        );

        if (hit.collider != null)
        {
            DeActivateBullet();
            bFired = false;
            BulletEffectIsFinished();

            return true;
        }

        return false;
    }

    private void ApplyDamage(Collider2D other)
    {
        directHitObject = other;

        // 데미지 처리
        effectComponent.PlayImpactEffect();

        IDamageable hit = other.GetComponent<IDamageable>();

        bool bCritical = false;

        int critical = UnityEngine.Random.Range(0, 100);

        float totalDamage = characterStatProvider.resultDamage;

        if (critical < characterStatProvider.criticalChance)
        {
            bCritical = true;
            totalDamage = characterStatProvider.totalDamage * 2 * characterStatProvider.totalDamageValue;
        }

        if (hit != null)
        {
            hit.TakeDamage(totalDamage, bCritical);
            hit.ApplyWeakness(characterStatProvider.weaknessTurnCnt);
            ApplyKnockBack(hit, other.transform.position);
        }
    }

    private void ApplyKnockBack(IDamageable enemy, Vector2 enemyPos)
    {
        Vector2 dir = enemyPos - (Vector2)transform.position;

        enemy.KnockBack(dir.normalized, knockBackPower);
    }

    public void Fire(Vector2 dir)
    {
        ActivateBullet();

        bFired = true;

        dir.Normalize();
        flyDir = dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        prevPosition = transform.position;
    }

    public void BulletEffectIsFinished()
    {
        BulletEffectIsFinishedEvent?.Invoke();
    }

    private void DeActivateBullet()
    {
        sr.gameObject.SetActive(false);
        effectComponent.gameObject.SetActive(false);

        circleCollider.enabled = false;
        explosionRangeCollider.enabled = false;
    }

    private void ActivateBullet()
    {
        sr.gameObject.SetActive(true);
        effectComponent.gameObject.SetActive(true);

        circleCollider.enabled = true;
        explosionRangeCollider.enabled = true;
    }
}
