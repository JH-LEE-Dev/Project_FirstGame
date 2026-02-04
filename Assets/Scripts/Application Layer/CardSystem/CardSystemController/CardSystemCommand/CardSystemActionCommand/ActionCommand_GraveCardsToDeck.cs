using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/GraveCardsToDeck")]
public class ActionCommand_GraveCardsToDeck : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.GraveCardsToDeck;
    }

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.GraveCardsToDeck(cards.AsSpan<CardDataInstance>().Slice(0,cnt));
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
