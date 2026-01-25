using UnityEngine;
using System.Collections.Generic;

public interface ICombatEffectReceiver
{
    void ApplyAttackModifier(float bonusDamage);

    void ApplyRangeModifier(float bonusRange);

    void ApplyCriticalChanceModifier(int bonusChance);

    void ApplyWeaknessModifier(int turnCnt);
}
