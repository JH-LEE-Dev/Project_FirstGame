using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIPlayerStat
{
    public PlayerStatType type;
    public UIStat_Unit unit;
    public Sprite iconImage;
}

public class UIStat_Player : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private List<UIPlayerStat> units = new List<UIPlayerStat>(10);

    public void Setup(PlayerStatType type, string title, float value)
    {
        UIPlayerStat dataInstance = units[(int)type];
        if (null == dataInstance)
            return;

        dataInstance.unit.Setup(dataInstance.iconImage, title, value);
    }

    public void Setup(PlayerStatType type, string title, string value)
    {
        UIPlayerStat dataInstance = units[(int)type];
        if (null == dataInstance)
            return;

        dataInstance.unit.Setup(dataInstance.iconImage, title, value);
    }

    public void ChangeValue(PlayerStatType type, float _current)
    {
        if (units.Count < (int)type)
            return;

        units[(int)type].unit.ValueChange(_current);
    }
}
