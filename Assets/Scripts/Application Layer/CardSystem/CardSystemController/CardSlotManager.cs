using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class CardSlotManager : ICardSlotSystemActionCommandHandler
{
    public event Action<int> CardSlotCntChangedEvent;

    private const int defaultSlotCnt = 2;
    private const int maxSlotCnt = 5;
    private const int maxSlotCardCnt = SYSTEM_VAR.maxDeckPileCount;
    private List<List<CardDataInstance>> bulletCardSlot = new List<List<CardDataInstance>>(maxSlotCnt);
    private List<List<CardDataInstance>> bulletCardSlotForUse = new List<List<CardDataInstance>>(maxSlotCnt);
    private List<List<CardDataInstance>> prevBulletCardSlot = new List<List<CardDataInstance>>(maxSlotCnt);
    private int bulletCardSlotCnt = 2;
    private int prevUsedBulletCardCnt = 0;

    public void Initialize()
    {
        for (int i = 0; i < maxSlotCnt; ++i)
        {
            prevBulletCardSlot.Add(new List<CardDataInstance>(maxSlotCardCnt));
            bulletCardSlot.Add(new List<CardDataInstance>(maxSlotCardCnt));
            bulletCardSlotForUse.Add(new List<CardDataInstance>(maxSlotCardCnt));
        }
    }

    public BulletCardUsedResult InsertCardToSlot(CardDataInstance usedCard)
    {
        BulletCardUsedResult result = new BulletCardUsedResult();

        CardData usedCardData = usedCard.GetCardData();

        for (int i = 0; i < bulletCardSlotCnt; ++i)
        {
            if (bulletCardSlot[i].Count == 0)
            {
                bulletCardSlot[i].Add(usedCard);
                prevBulletCardSlot[i].Add(usedCard);
                result.bVerified = true;
                result.slotIdx = i;
                ++prevUsedBulletCardCnt;
                SynchronizeCardSlotForUse();
                return result;
            }

            CardData currentCardData = bulletCardSlot[i][0].GetCardData();

            if (currentCardData.id == usedCardData.id && usedCardData.usingType == UsingType.Nesting)
            {
                bulletCardSlot[i].Add(usedCard);
                prevBulletCardSlot[i].Add(usedCard);
                result.bVerified = true;
                result.slotIdx = i;
                ++prevUsedBulletCardCnt;
                SynchronizeCardSlotForUse();
                return result;
            }
        }

        result.bVerified = false;
        result.slotIdx = -1;
        Debug.Log("Ä«µå ½½·ÔÀÌ °¡µæ Ã¡½À´Ï´Ù.");

        return result;
    }

    private void SynchronizeCardSlotForUse()
    {
        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            bulletCardSlotForUse[i].Clear();

            for (int j = 0; j < bulletCardSlot[i].Count; ++j)
            {
                bulletCardSlotForUse[i].Add(bulletCardSlot[i][j]);
            }
        }
    }

    public void DiscardBulletCard(int slotIdx)
    {
        var slotCard = bulletCardSlot[slotIdx];

        for (int i = 0; i < bulletCardSlot[slotIdx].Count; ++i)
        {
            bulletCardSlot[slotIdx][i].ResetCardData();
        }

        prevUsedBulletCardCnt -= bulletCardSlot[slotIdx].Count;

        bulletCardSlot[slotIdx].Clear();
        prevBulletCardSlot[slotIdx].Clear();

        SynchronizeCardSlotForUse();
    }

    public List<CardDataInstance> GetBulletCardSpecificSlot(int idx)
    {
        return bulletCardSlot[idx];
    }

    public int GetPrevUsedBulletCardCnt()
    {
        return prevUsedBulletCardCnt;
    }

    public void ClearAllBulletCard()
    {
        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            for (int j = 0; j < bulletCardSlot[i].Count; ++j)
            {
                bulletCardSlot[i][j].ResetCardData();
            }

            bulletCardSlot[i].Clear();
            bulletCardSlotForUse[i].Clear();
        }

        SynchronizeCardSlotForUse();
    }

    public void ClearAllPrevBulletCard()
    {
        prevUsedBulletCardCnt = 0;
        for (int i = 0; i < prevBulletCardSlot.Count; ++i)
        {
            prevBulletCardSlot[i].Clear();
        }
    }

    public IReadOnlyList<IReadOnlyList<CardDataInstance>> GetCardSlot()
    {
        return bulletCardSlotForUse;
    }

    public void SortCardSlot()
    {
        var comparer = new CardListPriorityComparer();
        bulletCardSlotForUse.Sort(comparer);
    }

    public void ReverseSortCardSlot()
    {
        bulletCardSlotForUse.Reverse();
    }

    public void ExecuteCommand(CardSystemCommand command, bool bUndo)
    {
        if (bUndo == false)
            command.Execute(this);
        else
            command.Undo(this);
    }

    public IReadOnlyList<IReadOnlyList<CardDataInstance>> GetPrevUsedBulletCard()
    {
        return prevBulletCardSlot;
    }

    public IReadOnlyList<IReadOnlyList<CardDataInstance>> GetCurrentBulletCards()
    {
        return bulletCardSlot;
    }

    public void ApplySlotCntModifier(int cnt)
    {
        bulletCardSlotCnt += cnt;

        if(bulletCardSlotCnt > maxSlotCardCnt)
            bulletCardSlotCnt = maxSlotCardCnt;

        CardSlotCntChangedEvent?.Invoke(bulletCardSlotCnt);
    }

    public void ResetSlotCntModifier()
    {
        bulletCardSlotCnt = defaultSlotCnt;

        CardSlotCntChangedEvent?.Invoke(bulletCardSlotCnt);
    }

    public void SetCardSystemContext(CardSystemContextType cardSystemContextType)
    {

    }
}
