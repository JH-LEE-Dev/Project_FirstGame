using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/DuplicateCardsToHand")]
public class ActionCommand_DuplicateCardsToHand : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.DuplicateCardCardsToHand;
    }

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(cnt);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < cnt; ++i)
        {
            if (cards[i] != null)
            {
                writeBuffer[i] = cardSystemActionCommandHandler.CreateCard(cards[i].GetCardData().id);
                writeBuffer[i].SetUpgrade(cards[i].IsUpgraded());
            }
        }

        cardSystemActionCommandHandler.CardsToHand(writeBuffer);
    }
}
