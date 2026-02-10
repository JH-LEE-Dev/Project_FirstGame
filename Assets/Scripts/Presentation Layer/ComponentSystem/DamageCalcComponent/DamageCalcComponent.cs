using UnityEngine;

public class DamageCalcComponent : IPrismBoltDamageCalculator, IAquaBurstDamageCalculator
{
    private PStatComponent statComponent;
    private PCombatComponent combatComponent;

    public void Initialize(PStatComponent _statComponent,PCombatComponent _combatComponent)
    {
        statComponent = _statComponent;
        combatComponent = _combatComponent;
    }

    public float GetDefaultDamage()
    {
        return statComponent.resultDamage;
    }

    public float GetPrismEffectDamage()
    {
        var additionalAttackStat = combatComponent.additionalAttackStat;
        return statComponent.CalcResultDamage_Optional(additionalAttackStat.attack, additionalAttackStat.additionalAttackValue, additionalAttackStat.totalDamageValue);
    }

    public float GetAquaEffectDamage()
    {
        var additionalAttackStat = combatComponent.additionalAttackStat;
        return statComponent.CalcResultDamage_Optional(additionalAttackStat.attack, additionalAttackStat.additionalAttackValue, additionalAttackStat.totalDamageValue);
    }
}
