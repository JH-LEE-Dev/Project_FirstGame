using UnityEngine;

public struct AdditionalAttackData
{
    public DebuffElementData debuffData;
    public float resultDamage;
    public bool bCritical;
    public AdditionalAttackData(DebuffElementData _debuffData, float _resultDamage,bool _bCritical)
    {
        debuffData = _debuffData;
        resultDamage = _resultDamage;
        bCritical = _bCritical;
    }
}


