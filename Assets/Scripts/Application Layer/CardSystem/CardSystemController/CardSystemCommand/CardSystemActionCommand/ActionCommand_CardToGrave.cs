using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/CardToGrave")]
public class ActionCommand_CardToGrave : CardSystemActionCommand<ICardSystemActionCommandHandler>
{
    public List<CardDataInstance> toGraveCards = new List<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        using var rentalBuffer_ToGrave = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer_ToGrave = rentalBuffer_ToGrave.Span;
        using var rentalBuffer_Duplicate = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer_Duplicate = rentalBuffer_Duplicate.Span;

        int duplicatedCnt = 0;
        int graveCnt = 0;

        for (int i = 0; i < toGraveCards.Count; ++i)
        {
            if (toGraveCards[i].GetCardData().id == (int)CardName.Distortion)
            {
                CardDataInstance duplicatedCard = cardSystemActionCommandHandler.CreateCard(toGraveCards[i].GetCardData().id);
                writeBuffer_Duplicate[i] = duplicatedCard;
                ++duplicatedCnt;
            }

            ++graveCnt;
            writeBuffer_ToGrave[i] = toGraveCards[i];
        }

        cardSystemActionCommandHandler.CardsToGrave(writeBuffer_ToGrave.Slice(0,graveCnt));
        cardSystemActionCommandHandler.CardsToGrave(writeBuffer_Duplicate.Slice(0,duplicatedCnt));

        rentalBuffer_ToGrave.Dispose();
        rentalBuffer_Duplicate.Dispose();

        toGraveCards.Clear();
    }
}
