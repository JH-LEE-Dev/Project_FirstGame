using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/Normal_Hit")]
public class Normal_Hit : BulletBehavior
{
    //필요하다면 사용.
    //public void Initialize(Bullet _bullet, ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider)
    //{
    //    base.Initialize(_bullet, _characterStatProvider, _bulletEffectProvider);
    //}

    public override void Enter()
    {
        bBehaviorEnd = false;
        CheckExplosion();
    }

    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        End();
    }

    public override void End()
    {
        bBehaviorEnd = true;
        BulletBehaviorEndEvent?.Invoke();
    }

    protected void ApplyDamage(Collider2D other) //적에게 데미지를 입히는 함수.
    {
        directHitObject = other;

        // 데미지 처리
        bullet.effectComponent.PlayImpactEffect();

        IDamageable hit = other.GetComponent<IDamageable>();

        bool bCritical = false;
        characterStatProvider.CalcBaseDamage(out bCritical);

        if (hit != null)
        {
            hit.TakeDamage(baseDamage, bCritical);
            hit.ApplyWeakness(characterStatProvider.weaknessTurnCnt);
            ApplyKnockBack(hit, other.transform.position);
        }
    }

    private void CheckExplosion() // 총알이 적에 맞았을 시, 범위 데미지 적용을 위한 함수.
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(bullet.transform.position,
            bullet.range + bullet.range * (characterStatProvider.attackRange * 0.01f), bullet.targetMask);

        foreach (var enemy in hitEnemies)
        {
            if (enemy == directHitObject)
                continue;

            ApplyDamage(enemy);
        }
    }

    private void ApplyKnockBack(IDamageable enemy, Vector2 enemyPos) //직격,범위 데미지에 맞은 적들을 넉백시키는 함수.
    {
        Vector2 dir = enemyPos - (Vector2)bullet.transform.position;

        enemy.KnockBack(dir.normalized, knockBackPower);
    }
}