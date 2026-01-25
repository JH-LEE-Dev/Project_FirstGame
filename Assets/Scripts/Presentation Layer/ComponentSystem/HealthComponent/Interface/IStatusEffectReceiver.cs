using UnityEngine;

public interface IStatusEffectReceiver
{
    void ApplyShieldModifier(float bonusShield);
    void IncreaseHP(float amount);
}
