using Mono.Cecil.Cil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public event Action BulletEffectIsFinishedEvent;

    //statComponent로 기능 분리할 것.
    [SerializeField] float speed = 1f;
    [SerializeField] float knockBackPower = 1f;
    [SerializeField] float range = 1f;
    [SerializeField] float attack = 1f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask outOfRangeMask;

    private EffectComponent effectComponent;

    private SpriteRenderer sr;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private CircleCollider2D explosionRangeCollider;

    private Vector2 flyDir;
    private Vector2 prevPosition;

    private bool bFired = false;

    private float attackModifier = 0;
    private float rangeModifier = 0;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        effectComponent = GetComponentInChildren<EffectComponent>();

        circleCollider.enabled = false;
        explosionRangeCollider.enabled = false;
        range = explosionRangeCollider.radius;
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

        if (CheckCollision_Enemy(delta,distance) == true)
            return;

        if (CheckCollision_OutofRange(delta, distance) == true)
            return;

        transform.position = currentPosition;
        prevPosition = currentPosition;
    }

    private bool CheckCollision_Enemy(Vector2 delta,float distance)
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
            sr.gameObject.SetActive(false);

            CheckExplosion();

            ResetModifier();

            return true;
        }

        return false;
    }

    private void CheckExplosion()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 
            explosionRangeCollider.radius, targetMask);

        foreach (var enemy in hitEnemies)
        {
            ApplyDamage(enemy);
        }
    }

    private bool CheckCollision_OutofRange(Vector2 delta,float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            prevPosition,
            delta.normalized,
            distance,
            outOfRangeMask
        );

        if (hit.collider != null)
        {
            sr.gameObject.SetActive(false);
            bFired = false;
            BulletEffectIsFinishedEvent?.Invoke();

            return true;
        }

        return false;
    }

    private void ApplyDamage(Collider2D other)
    {
        // 데미지 처리
        effectComponent.PlayImpactEffect();

        IDamageable hit = other.GetComponent<IDamageable>();

        if (hit != null)
        {
            hit.TakeDamage(attack + attackModifier);
            ApplyKnockBack(hit, other.transform.position);
        }
    }

    private void ApplyKnockBack(IDamageable enemy,Vector2 enemyPos)
    {
        Vector2 dir = enemyPos - (Vector2)transform.position;

        enemy.KnockBack(dir.normalized, explosionRangeCollider.radius);
    }

    public void Fire(Vector2 dir)
    {
        sr.gameObject.SetActive(true);
        effectComponent.gameObject.SetActive(true);

        bFired = true;

        dir.Normalize();
        flyDir = dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        prevPosition = transform.position;
    }

    public void ApplyAttackModifier(float bonusDamage)
    {
        attackModifier += bonusDamage;
    }

    public void ApplyRangeModifier(float bonusRange)
    {
        bonusRange = range * bonusRange;

        rangeModifier += bonusRange;
    }
    public void BulletEffectIsFinished()
    {
        BulletEffectIsFinishedEvent?.Invoke();

        ResetModifier();
    }

    public void ResetModifier()
    {
        attackModifier = 0;
        rangeModifier = 0;

        explosionRangeCollider.radius = range;
    }
}
