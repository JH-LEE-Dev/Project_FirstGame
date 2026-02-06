using UnityEngine;
using System.Collections.Generic;

public interface ICombatEffectReceiver
{
    void ApplyAdditionalAttackModifier(float bonusDamage);
    void ApplyAttackModifier(float bonusDamage);

    void ApplyRangeModifier(float bonusRange);

    void ApplyCriticalChanceModifier(int bonusChance);

    void ApplyWeaknessModifier(int turnCnt);

    void ApplyAttackCntModifier(int cnt);
    void ApplyTotalDamageModifier(float bonusDamage);
    void ApplyTotalDamageValueModifier(float bonusValue);
}
