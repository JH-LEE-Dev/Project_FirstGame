using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/UsedCardsRemoveFromHand")]
public class ActionCommand_UsedCardsRemoveFromHand : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.UsedCardsRemoveFromHand;
    }

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.CardsRemoveFromHand(cards.AsSpan<CardDataInstance>().Slice(0, cnt));
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
