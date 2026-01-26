using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/UsedCardsRemoveFromHand")]
public class ActionCommand_UsedCardsRemoveFromHand : CardSystemActionCommand<ICardSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.UsedCardsRemoveFromHand;
    }

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.CardsRemoveFromHand(cards.AsSpan<CardDataInstance>().Slice(0, cnt));
    }
}
