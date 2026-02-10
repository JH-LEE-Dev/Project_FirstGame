using UnityEngine;

public interface IAquaBurstDamageCalculator
{
    float GetDefaultDamage(out bool bCritical);

    AdditionalAttackData GetAquaEffectDamage();
}
