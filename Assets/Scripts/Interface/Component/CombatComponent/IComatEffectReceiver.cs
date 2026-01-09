using UnityEngine;
using System.Collections.Generic;

public interface ICombatEffectReceiver
{
    void ApplyAttackModifier(float bonusDamage);

    bool CanApplyBulletEffect();
}
