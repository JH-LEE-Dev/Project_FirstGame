using UnityEngine;

public class PStatComponent : StatComponent, ICombatEffectReceiver, ICharacterStatProvider
{
    public int attackCnt { get; private set; }
    public float attackRange { get; private set; }
    public float criticalChance { get; private set; }
    public float attack { get; private set; }
    public int weaknessTurnCnt { get; private set; }

    private int initialAttackCnt = 1;
    private float initialAttackRange = 0f;
    private float initialCriticalChange = 10f;
    private float initialAttack = 0f;

    public void Initialize()
    {
        attackCnt = initialAttackCnt;
        attack = initialAttack;
        criticalChance = initialCriticalChange;
        weaknessTurnCnt = 0;
        attackRange = initialAttackRange;
    }

    public void ApplyAttackModifier(float bonusDamage)
    {
        attack += bonusDamage;
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
    }

    public void DecreaseAttackCnt()
    {
        --attackCnt;
    }
}
