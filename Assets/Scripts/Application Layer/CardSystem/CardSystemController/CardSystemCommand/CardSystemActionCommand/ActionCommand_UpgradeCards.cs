using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardDataControlSystemAction/UpgradeCards")]
public class ActionCommand_UpgradeCards : CardSystemActionCommand<ICardDataControlSystemActionCommandHandler>
{
    [SerializeField] bool bPermanent = false;

    protected override void Execute(ICardDataControlSystemActionCommandHandler cardDataControlSystemActionCommandHandler)
    {
        cardDataControlSystemActionCommandHandler.UpgradeCards(cards.AsSpan<CardDataInstance>().Slice(0, cnt), bPermanent);
    }
}
