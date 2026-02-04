using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/HandToGrave")]
public class ActionCommand_HandToGrave : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        var handPile = cardSystemActionCommandHandler.GetHandPile();

        if (handPile.Count != 0)
        {
            using var rentalBuffer = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
            Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

            for (int i = 0; i < handPile.Count; ++i)
            {
                if (handPile[i].GetCardData().id == (int)CardName.Distortion)
                {
                    var card = cardSystemActionCommandHandler.CreateCard(handPile[i].GetCardData().id);
                    writeBuffer[i] = card;
                    writeBuffer[i].SetUpgrade(handPile[i].IsUpgraded());
                }
            }

            cardSystemActionCommandHandler.CardsToGrave(writeBuffer);
        }

        cardSystemActionCommandHandler.HandToGrave();
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
