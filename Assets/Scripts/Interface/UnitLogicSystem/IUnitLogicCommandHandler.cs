using UnityEngine;

public interface IUnitLogicCommandHandler
{

    void ApplyShieldModifier(float bonusShield);

    void ApplyAttackModifier(float bonusDamage);
}
