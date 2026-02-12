using UnityEngine;

public class DamageCalcComponent : IPrismBoltDamageCalculator, IAquaBurstDamageCalculator, IDamageSystem
{
    private PStatComponent statComponent;
    private PCombatComponent combatComponent;

    public void Initialize(PStatComponent _statComponent,PCombatComponent _combatComponent)
    {
        statComponent = _statComponent;
        combatComponent = _combatComponent;
    }

    public T GetDamageCalc<T>() where T : class
    {
        return this as T;
    }

    public float GetDefaultDamage(out bool bCritical)
    {
        return statComponent.CalcBaseDamage(out bCritical);
    }

    public AdditionalAttackData GetPrismEffectDamage()
    {
        bool bCritical = false;

        AdditionalAttackData data = new AdditionalAttackData(statComponent.additionalAttackStat.debuffData,
            statComponent.CalcResultDamage_Optional(out bCritical), bCritical);

        return data;
    }

    public AdditionalAttackData GetAquaEffectDamage()
    {
        bool bCritical = false;

        AdditionalAttackData data = new AdditionalAttackData(statComponent.additionalAttackStat.debuffData,
            statComponent.CalcResultDamage_Optional(out bCritical), bCritical);

        return data;
    }
}
