using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Strategy/BulletBehavior/PrismBolt/PrismBolt_Hit")]
public class PrismBolt_Hit : PrismBoltBehavior
{
    private Coroutine routine;
    private int subIndex;
    private float fixScale = 1f;

    public override void Initialize(Bullet owner, ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(owner, _characterStatProvider, _bulletEffectProvider, _damageSystem);
    }
    public override void Enter()
    {
        base.Enter();

        SetScale();

        var enemys = CheckRange(prismBolt.transform.position, UpscaleRange(prismBolt.originExplosionRange));
        foreach (var enemy in enemys)
        {
            bool bCritical = false;
            float damage = damageSystem.GetDamageCalc<IPrismBoltDamageCalculator>().GetDefaultDamage(out bCritical);

            ApplyDamage(enemy, damage, bCritical, prismBolt.transform.position);
            ApplyKnockBack(enemy, 2.8f);
        }

        if (routine != null)
            prismBolt.StopCoroutine(routine);
        routine = prismBolt.StartCoroutine(HitFxSequence());
    }

    public void SetScale()
    {
        fixScale = UpscaleRange(1f);
        prismBolt.bigFx.transform.localScale = new Vector2(fixScale, fixScale);
    }

    public override void Update()
    {
        if (bBehaviorEnd)
            return;
    }
    private IEnumerator HitFxSequence()
    {
        Vector2 center = prismBolt.transform.position;

        // 큰 폭발 이펙트 1회
        prismBolt.bigFx?.PlayAt(center);        
        
        // 1초 후
        yield return new WaitForSeconds(0.4f);

        // 0.2초 간격으로 5번 (1.5랜덤위치임)
        for (int i = 0; i < 8; i++)
        {
            Vector2 pos = center + Random.insideUnitCircle * (1.2f * fixScale);

            // 서브 이펙트 재생
            var fx = GetNextSubFx();
            if (fx != null)
            {
                fx.PlayAt(pos);
            }

            var subDamageData = damageSystem.GetDamageCalc<IPrismBoltDamageCalculator>().GetPrismEffectDamage();

            var targets = CheckRange(pos, UpscaleRange(prismBolt.originExplosionSubRange));
            foreach (var enemy in targets)
                ApplyAdditionalDamage(enemy, subDamageData, pos);

            yield return new WaitForSeconds(0.08f);
        }

        // 전부 끝나면 종료
        Exit();
    }

    private FxAutoHideOnAnimEnd GetNextSubFx()
    {
        if (prismBolt == null || prismBolt.subFx == null || prismBolt.subFx.Length == 0) return null;

        var fx = prismBolt.subFx[subIndex];
        subIndex = (subIndex + 1) % prismBolt.subFx.Length;
        fx.transform.localScale = new Vector2(fixScale, fixScale);
        return fx;
    }
    public override void Exit()
    {
        if (routine != null)
        {
            bullet.StopCoroutine(routine);
            routine = null;
        }

        base.Exit();
    }
}
