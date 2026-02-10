using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/ArcDischarge_Hit")]
public class ArcDischarge_Hit : ArcDischargeBehavior
{
    [SerializeField] private int maxTransference = 2;
    [SerializeField] private float finderRadius = 20f;

    private Queue<Collider2D> nextTargets = new(50);
    private HashSet<Collider2D> visits = new(50);

    public override void Enter()
    {
        bUpdateEnd = false;
        EnterHitEnemy(firstTarget, bullet.transform.position);
    }

    public override void Update()
    {
        if (true == bUpdateEnd)
            return;

    }

    public override void End()
    {
        bUpdateEnd = true;
        BulletBehaviorEndEvent?.Invoke();
    }

    public override void Exit()
    {
        bUpdateEnd = true;
        BulletEffectEndEvent?.Invoke();
    }

    #region Damage & Knockback

    protected void ApplyDamage(Collider2D other, Vector2 startPos)
    {
        Collider2D directHitObject = other;

        // 데미지 처리
        bullet.effectComponent.PlayImpactEffect();

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

    private void ApplyKnockBack(IDamageable enemy, Vector2 startPos, Vector2 enemyPos) //직격,범위 데미지에 맞은 적들을 넉백시키는 함수.
    {
        Vector2 dir = enemyPos - startPos;
        float tempPower = 1f;

        enemy.KnockBack(dir.normalized, tempPower);
    }

    private void EnterHitEnemy(Collider2D hit, Vector2 startPos, bool first = true)
    {
        if (null == hit)
            return;

        Vector2 targetPos = hit.transform.position;

        ApplyDamage(hit, startPos);

        if (first)
            CreateCircleCollder(hit);

        //이펙트 재생
        Debug.DrawLine(startPos, targetPos);
        //Sound.Play("Impact", bullet.transform.position);
    }

    #endregion

    #region BFS Enemy Search 

    private void CreateCircleCollder(Collider2D hitColl)
    {
        if (null == hitColl)
            return;

        nextTargets.Clear();
        visits.Clear();

        visits.Add(hitColl);
        nextTargets.Enqueue(hitColl);

        int _currentTransference = 0;
        while (0 < nextTargets.Count && _currentTransference < maxTransference)
            EnterTransference(ref _currentTransference);
    }

    private void EnterTransference(ref int _currentTransference)
    {
        int currentCnt = nextTargets.Count;

        for (int i = 0; i < currentCnt; ++i)
            ProcessOneEnemy();

        _currentTransference++;
    }

    private void ProcessOneEnemy()
    {
        Collider2D frontCollider = nextTargets.Dequeue();
        if (null == frontCollider)
            return;

        Vector2 _startPos = frontCollider.transform.position;

        Collider2D[] targets = Physics2D.OverlapCircleAll(_startPos, finderRadius, bullet.targetMask);
        if (0 >= targets.Length)
            return;

        foreach (Collider2D target in targets)
        {
            if (null == target || true == visits.Contains(target))
                continue;

            // 탐색 성공한 애들 바로 데미지 및 이펙트 연출
            EnterHitEnemy(target, _startPos, false);

            // 다음 대상 추가 및 방문자 추가
            nextTargets.Enqueue(target);
            visits.Add(target);
        }
    }

    #endregion
}
