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

public struct CardListPriorityComparer : IComparer<List<CardDataInstance>>
{
    public int Compare(List<CardDataInstance> x, List<CardDataInstance> y)
    {
        bool xEmpty = x == null || x.Count == 0;
        bool yEmpty = y == null || y.Count == 0;

        if (xEmpty && yEmpty) return 0;
        if (xEmpty) return 1;
        if (yEmpty) return -1;

        int xPriority = (int)x[0].GetCardData().priority;
        int yPriority = (int)y[0].GetCardData().priority;

        return xPriority.CompareTo(yPriority);
    }
}

public struct CardIdComparer : IComparer<CardDataInstance>
{
    public int Compare(CardDataInstance x, CardDataInstance y)
    {
        return x.GetCardData().id.CompareTo(y.GetCardData().id);
    }
}