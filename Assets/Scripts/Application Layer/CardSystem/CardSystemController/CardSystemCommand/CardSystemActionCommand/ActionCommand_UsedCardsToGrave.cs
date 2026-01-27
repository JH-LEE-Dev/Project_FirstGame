using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/UsedCardsToGrave")]
public class ActionCommand_UsedCardsToGrave : CardSystemActionCommand<ICardSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.UsedCardsToGrave;
    }

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        using var rentalBuffer_ToGrave = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer_ToGrave = rentalBuffer_ToGrave.Span;
        using var rentalBuffer_Duplicate = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer_Duplicate = rentalBuffer_Duplicate.Span;

        int duplicatedCnt = 0;
        int graveCnt = 0;

        for (int i = 0; i < cnt; ++i)
        {
            if (cards[i].GetCardData().id == (int)CardName.Distortion)
            {
                CardDataInstance duplicatedCard = cardSystemActionCommandHandler.CreateCard(cards[i].GetCardData().id);
                writeBuffer_Duplicate[i] = duplicatedCard;
                ++duplicatedCnt;
            }

            ++graveCnt;
            writeBuffer_ToGrave[i] = cards[i];
        }

        if (graveCnt != 0)
            cardSystemActionCommandHandler.CardsToGrave(writeBuffer_ToGrave.Slice(0, graveCnt));
        if (duplicatedCnt != 0)
            cardSystemActionCommandHandler.CardsToGrave(writeBuffer_Duplicate.Slice(0, duplicatedCnt));

        rentalBuffer_ToGrave.Dispose();
        rentalBuffer_Duplicate.Dispose();
    }
}
