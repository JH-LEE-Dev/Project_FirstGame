using UnityEngine;

public interface IStatusEffectCommandHandler : ICommandHandler
{

    void ApplyShieldModifier(float bonusShield);

    void ApplyAdditionalAttackModifier(float bonusDamage);
    void ApplyAttackModifier(float bonusDamage);

    void ApplyRangeModifier(float bonusRange);  

    void ApplyAttackCntModifier(int cnt);

    void ApplyCriticalChanceModifier(int chance);
    void ApplyWeaknessModifier(int turnCnt);
    void ApplyTotalDamageModifier(float bonusDamage);
    void ApplyTotalDamageValueModifier(float bonusValue);
    void UndoTotalDamageModifier(float bonusDamage);
    void HPDecrease(float amount);
    void HPIncrease(float amount);
    void SetCharacterCanAttackState(bool bCanAttack);
}
