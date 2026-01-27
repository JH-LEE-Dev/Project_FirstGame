using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/GraveCardsToDeck")]
public class ActionCommand_GraveCardsToDeck : CardSystemActionCommand<ICardSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.GraveCardsToDeck;
    }

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.GraveCardsToDeck(cards.AsSpan<CardDataInstance>().Slice(0,cnt));
    }
}
