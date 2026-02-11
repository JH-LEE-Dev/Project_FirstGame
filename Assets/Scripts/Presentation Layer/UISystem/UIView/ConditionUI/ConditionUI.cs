using System;
using System.Collections.Generic;
using UnityEngine;

public class ConditionUI : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Dictionary<DebuffElementEffectType, Sprite> initDatas;
    [SerializeField] private List<ConditionUI_Unit> units;

    public void UpdateConditions(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> currDebuffs)
    {
        ClearUnits();

        int i = 0;

        foreach (var item in currDebuffs)
        {
            DebuffElementEffectType tpye = item.Key;
            int remainCnt = item.Value.turnCnt;

            if (i < units.Count && initDatas.TryGetValue(tpye, out Sprite getSprite))
            {
                units[i].gameObject.SetActive(true);
                units[i].UpdateUnit(getSprite, remainCnt);
            }
        }
    }

    private void ClearUnits()
    {
        if (null != units)
        {
            foreach (ConditionUI_Unit unit in units)
            {
                unit.gameObject.SetActive(false);
            }
        }
    }
}
