using UnityEngine;

public interface ICardStatusEffectCommandHandler : ICommandHandler
{

    void ApplyShieldModifier(float bonusShield);

    void ApplyAttackModifier(float bonusDamage);

    void ApplyRangeModifier(float bonusRange);  

    void ApplyAttackCntModifier(int cnt);

    void ApplyCriticalChanceModifier(int chance);
    void ApplyWeaknessModifier(int turnCnt);

    void HPDecrease(float amount);
    void HPIncrease(float amount);
}
