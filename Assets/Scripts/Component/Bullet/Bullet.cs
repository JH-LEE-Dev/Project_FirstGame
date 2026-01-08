using Mono.Cecil.Cil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public event Action BulletEffectIsFinishedEvent;

    [SerializeField] float speed = 1f;
    [SerializeField] float attack = 1f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask outOfRangeMask;

    private EffectComponent effectComponent;

    private SpriteRenderer sr;
    private CircleCollider2D circleCollider;

    private Vector2 flyDir;
    private Vector2 prevPosition;

    private bool bFired = false;

    private int currentOccupiedSlotCnt = 0;
    private int effectSlotCnt = 2;
    bool bCanApplyEffect = true;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        effectComponent = GetComponentInChildren<EffectComponent>();
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
        RaycastHit2D hit = Physics2D.Raycast(
            prevPosition,
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

            return true;
        }

        return false;
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

        Unit hit = other.GetComponent<Unit>();

        if (hit != null)
        {
            hit.TakeDamage(attack);
        }
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
        attack += bonusDamage;

        IncreaseOccupiedSlotCnt();
    }

    public void IncreaseOccupiedSlotCnt()
    {
        ++currentOccupiedSlotCnt;

        if (currentOccupiedSlotCnt >= effectSlotCnt)
        {
            bCanApplyEffect = false;
        }
    }

    public bool CanApplyBulletEffect()
    {
        return bCanApplyEffect;
    }

    public void BulletEffectIsFinished()
    {
        BulletEffectIsFinishedEvent?.Invoke();
    }
}
