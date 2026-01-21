using UnityEngine;

public interface ICardStatusEffectCommandHandler
{

    void ApplyShieldModifier(float bonusShield);

    void ApplyAttackModifier(float bonusDamage);

    void ApplyRangeModifier(float bonusRange);  

    void ApplyAttackCntModifier(int cnt);
}
