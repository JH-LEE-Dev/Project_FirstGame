using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/ArcDischarge/Hit")]
public class ArcDischarge_Hit : ArcDischargeBehavior
{
    private Vector2 tempPos;
    private Coroutine routine;

    public override void Enter()
    {
        bBehaviorEnd = false;

        EnterHitEnemy(arcDischarge.firstTarget, bullet.initPosition, arcDischarge.firstHitPoint);
    }

    public override void Update()
    {
        if (true == bBehaviorEnd)
            return;
    }

    public override void End()
    {
        base.End();
    }

    public override void Exit()
    {
        base.Exit();

        if (null != routine)
        {
            bullet?.StopCoroutine(routine);
            routine = null;
        }
    }

    #region Damage & Knockback

    protected void ApplyDamage(Collider2D other, Vector2 startPos, Vector2 hitPoint)
    {
        IDamageable hit = other.GetComponent<IDamageable>();

        bool bCritical = false;
        float baseDamage = damageSystem.GetDamageCalc<IPrismBoltDamageCalculator>().GetDefaultDamage(out bCritical);

        base.ApplyDamage(other, baseDamage, bCritical, hitPoint);

        ApplyKnockBack(hit, startPos, other.transform.position);
    }

    private void ApplyKnockBack(IDamageable enemy, Vector2 startPos, Vector2 enemyPos)
    {
        Vector2 dir = enemyPos - startPos;
        float tempPower = 1f;

        enemy.KnockBack(dir.normalized, tempPower);
    }

    private void EnterHitEnemy(Collider2D hit, Vector2 startPos, Vector2 hitPoint, bool first = true)
    {
        if (null == hit)
            return;

        tempPos = startPos;

        Vector2 targetPos = hit.transform.position;

        ApplyDamage(hit, startPos, hitPoint);

        if (first)
            CreateCircleCollder(hit);

        //이펙트 재생
        Debug.DrawLine(startPos, targetPos, Color.red, 3f);
        arcDischarge?.PlayVFX(startPos, targetPos);
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
            while (0 < nextTargets.Count && currentTransferStep < arcDischarge.maxTransference)
            {
                yield return new WaitForSeconds(arcDischarge.chainDelay);

                if (bullet == null || !bullet.gameObject.activeInHierarchy)
                    yield break;

                int currentWaveCount = nextTargets.Count;

                for (int i = 0; i < currentWaveCount; i++)
                    ProcessOneEnemy(nextTargets, visits);

                currentTransferStep++;
            }

            // 이펙트 끝나는 딜레이를 여기서 해줘야 함
            yield return new WaitForSeconds(arcDischarge.chainDelay * 2);
        }
        finally
        {
            CollectionPool<Collider2D>.ReturnCollection(nextTargets);
            CollectionPool<Collider2D>.ReturnCollection(visits);

            // 지금 이거 끝나자마자 바로 호출 돼서 불릿이 비활성화 되는 건지 이펙트 마지막 전이가 안 나오고 있음
            // TODO: 나중에 해결
            Exit();
        }
    }

    private void CreateCircleCollder(Collider2D hitColl)
    {
        if (hitColl == null) 
            return;

        if (null != routine)
        {
            bullet.StopCoroutine(routine);
            routine = null;
        }

        routine = bullet.StartCoroutine(ChainLightningRoutine(hitColl));
    }

    private void ProcessOneEnemy(Queue<Collider2D> nextTargets, HashSet<Collider2D> visits)
    {
        Collider2D startCollider = nextTargets.Dequeue();
        if (null == startCollider)
            return;

        Vector2 _startPos = startCollider.transform.position;

        float currentRadius = UpscaleRange(arcDischarge.finderRadius);

        DrawDebugCircle(_startPos, currentRadius, Color.red, 3f); // TODO: 추후 제거

        Collider2D[] targets = Physics2D.OverlapCircleAll(_startPos, currentRadius, bullet.targetMask);
        if (0 >= targets.Length)
            return;

        foreach (Collider2D target in targets)
        {
            if (null == target || true == visits.Contains(target))
                continue;

            Vector2 dir = _startPos - (Vector2)target.transform.position;
            dir.Normalize();

            Vector2 hitPoint = (Vector2)target.transform.position + (dir * 0.5f);

            // 탐색 성공한 애들 바로 데미지 및 이펙트 연출
            EnterHitEnemy(target, _startPos, hitPoint, false);

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
