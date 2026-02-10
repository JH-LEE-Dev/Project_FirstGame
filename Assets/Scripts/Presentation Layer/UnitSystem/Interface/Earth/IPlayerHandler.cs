using System.Collections.Generic;
using UnityEngine;

public interface IPlayerHandler
{
    Transform GetTransform();
    float GetMaxHealth();
    float GetCurrentHealth();
    float GetCurrentShield();

    float GetPrevHealth();

    float GetPrevShield();
    int GetPlayerCurrentMoney();
    IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> currentAppliedDebuff { get; }
    void ClearDebuff();
}
