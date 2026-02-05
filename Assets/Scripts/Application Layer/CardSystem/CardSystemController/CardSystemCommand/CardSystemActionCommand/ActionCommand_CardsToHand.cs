using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/CardsToHand")]
public class ActionCommand_CardsToHand : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{
    protected override void Execute(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {
        var handPile = cardLogicSystemActionCommandHandler.GetHandPile();

        if(handPile.Count + cnt > SYSTEM_VAR.maxHandPileCount)
            cnt = SYSTEM_VAR.maxHandPileCount - handPile.Count;

        if(cnt < 0)
        {
            Debug.LogWarning("패로 카드를 이동시키지 못했습니다. 패 총량 초과.");
            return;
        }

        cardLogicSystemActionCommandHandler.CardsToHand(cards.AsSpan<CardDataInstance>().Slice(0, cnt));
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {

    }
}
