using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/CardToExtinction")]
public class ActionCommand_CardToExtinction : CardSystemActionCommand<ICardSystemActionCommandHandler>
{
    public List<CardDataInstance> toExtinctionCards = new List<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < toExtinctionCards.Count; ++i)
        {
            writeBuffer[i] = toExtinctionCards[i]; 
        }

        cardSystemActionCommandHandler.CardsToExtinction(writeBuffer.Slice(0, toExtinctionCards.Count));

        rentalBuffer.Dispose();

        toExtinctionCards.Clear();
    }
}
