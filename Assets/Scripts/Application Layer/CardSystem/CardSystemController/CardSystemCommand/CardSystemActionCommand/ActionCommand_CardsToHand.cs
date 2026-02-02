using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/CardsToHand")]
public class ActionCommand_CardsToHand : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{
    [SerializeField] bool bPermanent = false;

    protected override void Execute(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {
        cardLogicSystemActionCommandHandler.CardsToHand(cards.AsSpan<CardDataInstance>().Slice(0, cnt));
    }
}
