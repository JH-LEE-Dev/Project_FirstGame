using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/SlotCardsToGrave")]
public class ActionCommand_SlotCardsToGrave : CardSystemActionCommand<ICardSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.SlotCardsToGrave;
    }

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.CardsToGrave(cards.AsSpan<CardDataInstance>().Slice(0, cnt));
    }
}
