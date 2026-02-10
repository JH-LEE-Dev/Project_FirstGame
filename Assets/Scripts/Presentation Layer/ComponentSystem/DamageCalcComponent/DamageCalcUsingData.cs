using UnityEngine;

public struct AdditionalAttackData
{
    DebuffElementData debuffData;
    float resultDamage;
    public AdditionalAttackData(DebuffElementData _debuffData, float _resultDamage)
    {
        debuffData = _debuffData;
        resultDamage = _resultDamage;
    }
}


