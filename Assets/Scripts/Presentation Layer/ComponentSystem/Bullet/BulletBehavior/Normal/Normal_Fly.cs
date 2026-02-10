using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/Normal_Fly")]
public class Normal_Fly : NormalBehavior
{
    public override void Enter()
    {
        bBehaviorEnd = false;
        prevPosition = bullet.prevPosition;
    }

    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        Vector2 currentPosition = (Vector2)bullet.transform.position + bullet.flyDir * speed * Time.deltaTime;
        bullet.transform.position = currentPosition;
        Vector2 delta = currentPosition - prevPosition;
        float distance = delta.magnitude;

        if (CheckCollision_Enemy(delta, distance) == true) //총알에 직격한 적이 있는지 체크.
            return;

        if (CheckCollision_OutofRange(delta, distance) == true) //총알이 범위를 벗어났는지 체크.
            return;

        bullet.transform.position = currentPosition;
        prevPosition = currentPosition;
    }

    public override void End()
    {
        BulletBehaviorEndEvent?.Invoke();
        bBehaviorEnd = true;
    }

    public override void Exit()
    {
        BulletEffectEndEvent?.Invoke();
    }

    private bool CheckCollision_Enemy(Vector2 delta, float distance) //총알에 직격한 적이 있는지 체크.
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            prevPosition,
            bullet.range,
            delta.normalized,
            distance,
            bullet.targetMask
        );

        if (hit.collider != null)
        {
            ApplyDamage(hit.collider);

            Sound.Play("Impact", bullet.transform.position);

            End();

            return true;
        }

        return false;
    }

    private bool CheckCollision_OutofRange(Vector2 delta, float distance) //총알이 범위를 벗어났는지 체크.
    {
        RaycastHit2D hit = Physics2D.Raycast(
            prevPosition,
            delta.normalized,
            distance,
            bullet.outOfRangeMask
        );

        if (hit.collider != null)
        {
            Exit();

            return true;
        }

        return false;
    }

    protected void ApplyDamage(Collider2D other) //적에게 데미지를 입히는 함수.
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
        Vector2 dir = enemyPos - (Vector2)bullet.transform.position;

        enemy.KnockBack(dir.normalized, knockBackPower);
    }
}