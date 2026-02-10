using UnityEngine;

public interface IPrismBoltDamageCalculator
{
    float GetDefaultDamage(out bool bCritical);

    AdditionalAttackData GetPrismEffectDamage();
}
