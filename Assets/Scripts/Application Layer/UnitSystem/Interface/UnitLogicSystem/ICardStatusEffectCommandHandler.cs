using UnityEngine;

public interface ICardStatusEffectCommandHandler
{

    void ApplyShieldModifier(float bonusShield);

    void ApplyAttackModifier(float bonusDamage);

    void AttackAgain();
}
