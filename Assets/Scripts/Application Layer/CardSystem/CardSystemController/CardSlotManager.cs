using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CardSlotManager : ICardSlotSystemActionCommandHandler
{
    private List<CardDataInstance> bulletCardSlot = new List<CardDataInstance>(30);
    private int bulletCardSlotCnt = 2;

    public void Initialize()
    {

    }

    public BulletCardUsedResult InsertCardToSlot(CardDataInstance usedCard)
    {
        BulletCardUsedResult result = new BulletCardUsedResult();

        CardData usedCardData = usedCard.GetCardData();

        for (int i = 0; i < bulletCardSlotCnt; ++i)
        {
            if (i >= bulletCardSlot.Count)
            {
                bulletCardSlot.Add(usedCard);
                result.bVerified = true;
                result.slotIdx = i;

                return result;
            }

            CardData currentCardData = bulletCardSlot[i].GetCardData();

            if (currentCardData.id == usedCardData.id && usedCardData.usingType == UsingType.Nesting)
            {
                ++bulletCardSlot[i].nestingCnt;
                result.bVerified = true;
                result.slotIdx = i;

                return result;
            }
        }

        result.bVerified = false;
        result.slotIdx = -1;
        Debug.Log("Ä«µå ½½·ÔÀÌ ±×µæ Ã¡½À´Ï´Ù.");

        return result;
    }

    public void DiscardBulletCard(int slotIdx)
    {
        var slotCard = bulletCardSlot[slotIdx];
        slotCard.ResetCardData();
        bulletCardSlot.RemoveAt(slotIdx);
    }

    public void ClearAllBulletCard()
    {
        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            bulletCardSlot[i].ResetCardData();
        }

        bulletCardSlot.Clear();
    }

    public IReadOnlyList<CardDataInstance> GetCardSlot()
    {
        return bulletCardSlot;
    }

    public void SortCardSlot()
    {
        var comparer = new CardEffectPriorityComparer();
        bulletCardSlot.Sort(comparer);
    }

    public void ApplyValueModifier(int valueModifier)
    {
        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            if (bulletCardSlot[i].GetCardData().usingType == UsingType.Nesting)
            {
                bulletCardSlot[i].valueModifier *= valueModifier;
            }
        }
    }

    public void ExecuteCommand(CardSystemCommand command)
    {
        command.Execute(this);
    }
}
