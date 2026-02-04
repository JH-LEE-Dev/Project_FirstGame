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
        cardSystemActionCommandHandler.GraveCardsToHand(cards.AsSpan<CardDataInstance>().Slice(0, cnt));
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
