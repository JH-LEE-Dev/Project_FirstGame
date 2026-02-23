using UnityEngine;
[CreateAssetMenu(menuName = "Strategy/BulletBehavior/AquaBurst/AquaBurst_Hit")]

public class AquaBurst_Hit : AquaBurstBehavior
{

    public override void Enter()
    {
        base.Enter();

        int count = CheckSector(aquaBurst.transform.position,
            UpscaleRange(aquaBurst.originExplosionRange),
            60f,
            aquaBurst.initDir);


        Collider2D directHit = aquaBurst.directHitEnemy;

        for (int i = 0; i < count; i++)
        {
            var enemy = sectorResultBuffer[i];

            if (directHit != null && directHit == enemy) 
                continue;
            var damageData = damageSystem
                .GetDamageCalc<IAquaBurstDamageCalculator>()
                .GetAquaEffectDamage();

            ApplyAdditionalDamage(enemy, damageData, (Vector2)enemy.transform.position - (aquaBurst.initDir * 0.1f));
            ApplyKnockBack(enemy, 4f);
        }

        float fixScale = UpscaleRange(1f);
        aquaBurst.bigFx.transform.localScale = new Vector2(fixScale, fixScale);
        PlayExplosionDirected(aquaBurst.bigFx, aquaBurst.transform.position, aquaBurst.initDir);
    }

    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        Exit();
    }


    public override void Exit()
    {
        aquaBurst.directHitEnemy = null;
        base.Exit();
    }

    private void PlayExplosionDirected(FxAutoHideOnAnimEnd fx, Vector2 pos, Vector2 dir)
    {
        if (fx == null) return;

        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.up;

        dir.Normalize();

        Vector2 baseDir = new Vector2(1f, 1f).normalized;

        float angle = Vector2.SignedAngle(baseDir, dir); 
        fx.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        fx.PlayAt(pos);
    }
}
