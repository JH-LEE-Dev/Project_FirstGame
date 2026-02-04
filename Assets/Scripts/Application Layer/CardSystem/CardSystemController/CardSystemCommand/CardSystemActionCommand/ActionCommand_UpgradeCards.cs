using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardDataControlSystemAction/UpgradeCards")]
public class ActionCommand_UpgradeCards : CardSystemActionCommand<ICardDataControlActionCommandHandler>
{
    [SerializeField] bool bPermanent = false;

    protected override void Execute(ICardDataControlActionCommandHandler cardDataControlSystemActionCommandHandler)
    {
        cardDataControlSystemActionCommandHandler.UpgradeCards(cards.AsSpan<CardDataInstance>().Slice(0, cnt), bPermanent);
    }

    protected override void Undo(ICardDataControlActionCommandHandler cardDataControlSystemActionCommandHandler)
    {

    }
}
