using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class DebuffDatas
{
    public DebuffElementEffectType type;
    public Sprite icon;
}

public class ConditionUI : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private List<ConditionUI_Unit> units;
    [SerializeField] private List<DebuffDatas> initDatas;

    private Dictionary<DebuffElementEffectType, Sprite> initDictionary = new();

    private void Awake()
    {
        initDictionary.Clear();

        foreach (var data in initDatas)
        {
            initDictionary.Add(data.type, data.icon);
        }
    }

    public void UpdateConditions(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> currDebuffs)
    {
        ClearUnits();

        if (null == currDebuffs)
            return;

        int i = 0;

        foreach (var item in currDebuffs)
        {
            DebuffElementEffectType tpye = item.Key;
            int remainCnt = item.Value.turnCnt;

            if (i < units.Count && initDictionary.TryGetValue(tpye, out Sprite getSprite))
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
