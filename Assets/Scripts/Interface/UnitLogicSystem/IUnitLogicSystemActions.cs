using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public interface IUnitLogicSystemActions
{
    void Initialize(Character character, List<Enemy> enemies);

    void ApplyShieldModifier(float bonusShield);

    void ApplyAttackModifier(float bonusDamage);

    bool CanApplyBulletEffect();
}
