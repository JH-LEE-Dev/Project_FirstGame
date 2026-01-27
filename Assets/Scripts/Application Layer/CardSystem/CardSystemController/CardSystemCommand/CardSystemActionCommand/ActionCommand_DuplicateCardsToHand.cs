using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/DuplicateCardsToHand")]
public class ActionCommand_DuplicateCardsToHand : CardSystemActionCommand<ICardSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.DuplicateCardCardsToHand;
    }

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(cnt);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < cnt; ++i)
        {
            if (cards[i] != null)
            {
                writeBuffer[i] = cardSystemActionCommandHandler.CreateCard(cards[i].GetCardData().id);
            }
        }

        cardSystemActionCommandHandler.CardsToHand(writeBuffer);

        rentalBuffer.Dispose();
    }
}
