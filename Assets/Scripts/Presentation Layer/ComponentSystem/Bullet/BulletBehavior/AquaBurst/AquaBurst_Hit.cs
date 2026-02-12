using UnityEngine;
[CreateAssetMenu(menuName = "Strategy/BulletBehavior/AquaBurst/AquaBurst_Hit")]

public class AquaBurst_Hit : AquaBurstBehavior
{

    public override void Enter()
    {
        base.Enter();

        int count = CheckSector(aquaBurst.transform.position, 
            aquaBurst.originExplosionRange,
            60f,
            aquaBurst.initDir);


        for (int i = 0; i < count; i++)
        {
            var enemy = sectorResultBuffer[i];

            bool bCritical;
            float damage = damageSystem
                .GetDamageCalc<IAquaBurstDamageCalculator>()
                .GetDefaultDamage(out bCritical);

            ApplyDamage(enemy, damage, bCritical);
            ApplyKnockBack(enemy, 4f);
        }

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
