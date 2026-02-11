using System;
using System.Collections.Generic;
using UnityEngine;

public class ConditionUI : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Dictionary<DebuffElementEffectType, Sprite> initDatas;
    [SerializeField] private List<ConditionUI_Unit> units;

    public void Init(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> currDebuffs)
    {
        if (null != units)
        {
            foreach (ConditionUI_Unit unit in units)
            {
                unit.gameObject.SetActive(false);
            }
        }

        // 여기서 키랑 

    }
}
