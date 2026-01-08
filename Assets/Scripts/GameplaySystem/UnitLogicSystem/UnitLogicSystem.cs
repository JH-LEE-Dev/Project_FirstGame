using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UnitLogicSystem : MonoBehaviour, IUnitLogicSystemProvider
{
    private Character characterUnit;
    private List<Enemy> enemyUnits;

    public void Initialize(Character _characterUnit, List<Enemy> _enemyUnits)
    {
        characterUnit = _characterUnit;
        enemyUnits = _enemyUnits;
    }

    public void ApplyShieldModifier(float bonusShield)
    {

    }

    public void ApplyAttackModifier(float bonusDamage)
    {
        characterUnit.combatEffectReceiver.ApplyAttackModifier(bonusDamage);
    }

    public bool CanApplyBulletEffect()
    {
        return characterUnit.combatEffectReceiver.CanApplyBulletEffect();
    }
}
