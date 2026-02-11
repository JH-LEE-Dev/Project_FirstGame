using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/ArcDischarge/Hit")]
public class ArcDischarge_Hit : ArcDischargeBehavior
{
    private Vector2 tempPos;

    public override void Enter()
    {
        bBehaviorEnd = false;

        EnterHitEnemy(arcDischarge.firstTarget, bullet.transform.position);
    }

    public override void Update()
    {
        if (true == bBehaviorEnd)
            return;
    }

    public override void End()
    {
        bBehaviorEnd = true;
        BulletBehaviorEndEvent?.Invoke();
    }

    public override void Exit()
    {
        bBehaviorEnd = true;
        BulletEffectEndEvent?.Invoke();
    }

    #region Damage & Knockback

    protected void ApplyDamage(Collider2D other, Vector2 startPos)
    {
        Collider2D directHitObject = other;

        // 데미지 처리
        //bullet.nonProjectileObj.effectComponent.PlayImpactEffect();

        IDamageable hit = other.GetComponent<IDamageable>();

        bool bCritical = false;
        float baseDamage = damageSystem.GetDamageCalc<IPrismBoltDamageCalculator>().GetDefaultDamage(out bCritical);

        if (hit != null)
        {
            hit.TakeDamage(baseDamage, bCritical, bulletEffectProvider.currentEffectElements);
            hit.ApplyWeakness(characterStatProvider.weaknessTurnCnt);
            ApplyKnockBack(hit, startPos, other.transform.position);
        }
    }

    private void ApplyKnockBack(IDamageable enemy, Vector2 startPos, Vector2 enemyPos)
    {
        Vector2 dir = enemyPos - startPos;
        float tempPower = 1f;

        enemy.KnockBack(dir.normalized, tempPower);
    }

    private void EnterHitEnemy(Collider2D hit, Vector2 startPos, bool first = true)
    {
        if (null == hit)
            return;

        tempPos = startPos;
        Debug.Log(tempPos);

        Vector2 targetPos = hit.transform.position;

        ApplyDamage(hit, startPos);

        if (first)
            CreateCircleCollder(hit);

        //이펙트 재생
        Debug.DrawLine(startPos, targetPos, Color.red, 3f);
        //Sound.Play("Impact", bullet.transform.position);
    }

    #endregion

    #region BFS Enemy Search 

    private IEnumerator ChainLightningRoutine(Collider2D firstTarget)
    {
        Queue<Collider2D> nextTargets = CollectionPool<Collider2D>.GetQueue(30);
        HashSet<Collider2D> visits = CollectionPool<Collider2D>.GetSet(30);

        visits.Add(firstTarget);
        nextTargets.Enqueue(firstTarget);

        int currentTransferStep = 0;

        try
        {
            while (0 < nextTargets.Count && currentTransferStep <= arcDischarge.maxTransference)
            {
                yield return new WaitForSeconds(arcDischarge.chainDelay);

                if (bullet == null || !bullet.gameObject.activeInHierarchy)
                    yield break;

                int currentWaveCount = nextTargets.Count;

                for (int i = 0; i < currentWaveCount; i++)
                    ProcessOneEnemy(nextTargets, visits);

                currentTransferStep++;
            }
        }
        finally
        {
            CollectionPool<Collider2D>.ReturnCollection(nextTargets);
            CollectionPool<Collider2D>.ReturnCollection(visits);

            Exit();
        }
    }

    private void CreateCircleCollder(Collider2D hitColl)
    {
        if (hitColl == null) 
            return;

        bullet.StartCoroutine(ChainLightningRoutine(hitColl));
    }

    private void ProcessOneEnemy(Queue<Collider2D> nextTargets, HashSet<Collider2D> visits)
    {
        Collider2D frontCollider = nextTargets.Dequeue();
        if (null == frontCollider)
            return;

        Vector2 _startPos = frontCollider.transform.position;

        // TODO: 추후 제거
        DrawDebugCircle(_startPos, arcDischarge.finderRadius, Color.red, 3f);

        Collider2D[] targets = Physics2D.OverlapCircleAll(_startPos, arcDischarge.finderRadius, bullet.targetMask);
        if (0 >= targets.Length)
            return;

        foreach (Collider2D target in targets)
        {
            if (null == target || true == visits.Contains(target))
                continue;

            // 탐색 성공한 애들 바로 데미지 및 이펙트 연출
            EnterHitEnemy(target, _startPos, false);

            nextTargets.Enqueue(target);
            visits.Add(target);
        }
    }

    private void DrawDebugCircle(Vector2 center, float radius, Color color, float duration)
    {
        int segments = 36;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float a1 = i * angleStep * Mathf.Deg2Rad;
            float a2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector2 p1 = center + new Vector2(Mathf.Cos(a1), Mathf.Sin(a1)) * radius;
            Vector2 p2 = center + new Vector2(Mathf.Cos(a2), Mathf.Sin(a2)) * radius;

            Debug.DrawLine(p1, p2, color, duration);
        }
    }

    #endregion
}
