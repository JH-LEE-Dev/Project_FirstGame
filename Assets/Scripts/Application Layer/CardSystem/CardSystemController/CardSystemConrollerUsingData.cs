using System.Collections.Generic;
using UnityEngine;

public struct CardUsedResult
{
    public bool bVerified;
    public int slotIdx;
    public CardDataInstance usedCard;
}

public struct BulletCardUsedResult
{
    public bool bVerified;
    public int slotIdx;
}

public struct CardEffectPriorityComparer : IComparer<CardDataInstance>
{
    public int Compare(CardDataInstance x, CardDataInstance y)
        => x.GetCardData().priority.CompareTo(y.GetCardData().priority);
}