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

            int duplicatedCnt = 0;

            for (int i = 0; i < handPile.Count; ++i)
            {
                if (handPile[i].GetCardData().id == (int)CardName.Distortion)
                {
                    var card = cardSystemActionCommandHandler.CreateCard(handPile[i].GetCardData().id);

                    if (card == null)
                    {
                        Debug.LogWarning("카드를 복사하지 못했습니다. 카드 총량 초과.");
                        break;
                    }

                    writeBuffer[duplicatedCnt] = card;
                    writeBuffer[duplicatedCnt].SetUpgrade(handPile[i].IsUpgraded());
                    ++duplicatedCnt;
                }
            }

            if (duplicatedCnt != 0)
                cardSystemActionCommandHandler.CardsToGrave(writeBuffer.Slice(0, duplicatedCnt));
        }

        cardSystemActionCommandHandler.HandToGrave();
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
