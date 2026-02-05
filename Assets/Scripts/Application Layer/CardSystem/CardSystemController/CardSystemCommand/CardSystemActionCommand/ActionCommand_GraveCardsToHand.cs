using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/GraveCardsToHand")]
public class ActionCommand_GraveCardsToHand : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.GraveCardsToHand;
    }

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        var handPile = cardSystemActionCommandHandler.GetHandPile();

        if (handPile.Count + cnt > SYSTEM_VAR.maxHandPileCount)
            cnt = SYSTEM_VAR.maxHandPileCount - handPile.Count;

        if (cnt < 0)
        {
            Debug.LogWarning("패로 카드를 이동시키지 못했습니다. 패 총량 초과.");
            return;
        }

        cardSystemActionCommandHandler.GraveCardsToHand(cards.AsSpan<CardDataInstance>().Slice(0, cnt));
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
