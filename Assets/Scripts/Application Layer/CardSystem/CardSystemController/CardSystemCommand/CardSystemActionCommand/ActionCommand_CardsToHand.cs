using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/CardsToHand")]
public class ActionCommand_CardsToHand : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{
    protected override void Execute(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {
        cardLogicSystemActionCommandHandler.CardsToHand(cards.AsSpan<CardDataInstance>().Slice(0, cnt));
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {

    }
}
