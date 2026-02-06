using UnityEngine;

public class PStatComponent : StatComponent, ICombatEffectReceiver, ICharacterStatProvider
{
    public int attackCnt { get; private set; }
    public float attackRange { get; private set; }
    public float criticalChance { get; private set; }
    public float attack { get; private set; }
    public int weaknessTurnCnt { get; private set; }
    public float totalDamage { get; private set; }
    public float resultDamage { get; private set; }
    public float totalDamageValue { get; private set; }

    private int initialAttackCnt = 1;
    private float initialAttackRange = 0f;
    private float initialCriticalChange = 10f;
    private float initialAttack = 0f;
    private float initialTotalDamageValue = 1f;
    private float additionalAttack = 0;

    public void Initialize()
    {
        attackCnt = initialAttackCnt;
        attack = initialAttack;
        criticalChance = initialCriticalChange;
        weaknessTurnCnt = 0;
        attackRange = initialAttackRange;
        totalDamageValue = initialTotalDamageValue;
    }

    public void ApplyAdditionalAttackModifier(float bonusDamage)
    {
        additionalAttack += bonusDamage;
        CalcResultDamage();
    }

    public void ApplyAttackModifier(float bonusDamage)
    {
        attack += bonusDamage;
        CalcResultDamage();
    }

    public void ApplyTotalDamageModifier(float bonusDamage)
    {
        additionalAttack *= bonusDamage;

        CalcResultDamage();
    }

    public void UndoTotalDamageModifier(float bonusDamage)
    {
        additionalAttack /= bonusDamage;
        CalcResultDamage();
    }

    public void ApplyTotalDamageValueModifier(float bonusValue)
    {
        totalDamageValue = bonusValue;
        CalcResultDamage();
    }

    public void CalcResultDamage()
    {
        totalDamage = attack + additionalAttack;
        resultDamage = totalDamage * totalDamageValue;
    }

    public void ApplyRangeModifier(float bonusRange)
    {
        attackRange += bonusRange;
    }

    public void ApplyCriticalChanceModifier(int bonusCriticalChance)
    {
        criticalChance += bonusCriticalChance;
    }

    public void ApplyAttackCntModifier(int bonusCnt)
    {
        attackCnt += bonusCnt;
    }

    public void ApplyWeaknessModifier(int turnCnt)
    {
        weaknessTurnCnt += turnCnt;
    }

    public void ResetStat()
    {
        attack = initialAttack;
        attackRange = initialAttackRange;
        criticalChance = initialCriticalChange;
        attackCnt = initialAttackCnt;
        weaknessTurnCnt = 0;
        additionalAttack = 0;
        totalDamageValue = initialTotalDamageValue;
        totalDamage = 0;
        resultDamage = 0;
    }

    public void DecreaseAttackCnt()
    {
        --attackCnt;
    }
}
