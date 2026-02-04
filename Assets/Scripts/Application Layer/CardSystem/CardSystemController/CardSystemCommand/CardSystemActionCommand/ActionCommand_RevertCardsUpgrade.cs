using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardDataControlSystemAction/RevertCardsUpgrade")]
public class ActionCommand_RevertCardsUpgrade : CardSystemActionCommand<ICardDataControlActionCommandHandler>
{
    [SerializeField] bool bPermanent = false;

    protected override void Execute(ICardDataControlActionCommandHandler cardDataControlSystemActionCommandHandler)
    {
        cardDataControlSystemActionCommandHandler.RevertCardsUpgrade(cards.AsSpan<CardDataInstance>().Slice(0, cnt), bPermanent);
    }
}
