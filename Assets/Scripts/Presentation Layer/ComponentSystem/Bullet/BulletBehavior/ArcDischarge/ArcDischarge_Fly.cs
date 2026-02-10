using UnityEngine;

public class ArcDischarge_Fly : ArcDischargeBehavior
{
    private Vector2 bulletStartPos = Vector2.zero;

    public override void Enter()
    {
        bUpdateEnd = false;
        bulletStartPos = bullet.transform.position;
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
        BulletEffectEndEvent?.Invoke();
    }


    private bool CheckCollision_Enemy() //총알에 직격한 적이 있는지 체크.
    {
        RaycastHit2D hit = Physics2D.Raycast(
            bullet.transform.position,
            bullet.flyDir,
            Mathf.Infinity,
            bullet.targetMask
        );

        if (null != hit.collider)
        {
            ApplyDamage(hit.collider);

            //Sound.Play("Impact", bullet.transform.position);
            //이펙트 재생

            End();

            return true;
        }

        return false;
    }

    private bool CheckCollision_OutofRange()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            bulletStartPos,
            bullet.flyDir,
            Mathf.Infinity,
            bullet.outOfRangeMask
        );

        if (null != hit.collider)
        {
            Exit();
            return true;
        }

        return false;
    }

    protected void ApplyDamage(Collider2D other)
    {
        directHitObject = other;

        // 데미지 처리
        bullet.effectComponent.PlayImpactEffect();

        IDamageable hit = other.GetComponent<IDamageable>();

        bool bCritical = false;
        baseDamage = damageSystem.GetDamageCalc<IPrismBoltDamageCalculator>().GetDefaultDamage(out bCritical);

        if (hit != null)
        {
            hit.TakeDamage(baseDamage, bCritical);
            hit.ApplyWeakness(characterStatProvider.weaknessTurnCnt);
            ApplyKnockBack(hit, other.transform.position);
        }
    }

    private void ApplyKnockBack(IDamageable enemy, Vector2 enemyPos) //직격,범위 데미지에 맞은 적들을 넉백시키는 함수.
    {
        Vector2 dir = enemyPos - bulletStartPos;

        enemy.KnockBack(dir.normalized, knockBackPower);
    }

    private void CreateCircleCollder(Vector2 hitPos)
    {

    }
}
