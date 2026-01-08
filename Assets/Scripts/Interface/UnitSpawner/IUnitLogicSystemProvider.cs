using UnityEngine;

public interface IUnitLogicSystemProvider
{
    void ApplyShieldModifier(float bonusShield);

    void ApplyAttackModifier(float bonusDamage);

    bool CanApplyBulletEffect();
}
